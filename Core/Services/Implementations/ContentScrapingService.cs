using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Shared.Dtos.Content;
using Services.Abstraction.Contracts;
using Services.Abstraction.Exceptions;

namespace Services.Implementations
{
    public class ContentScrapingService : IContentScrapingService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private const int MaxVideoRedirectDepth = 3;
        private const long MaxVideoBytes = 52428800; // 50 MB

        // ??? User-Agent ?????? ??????? ????? ????? ?????? ??? Block ????????
        private const string BrowserUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";

        public ContentScrapingService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        #region Universal Endpoint (Scrape Any Link Without Block)

        public async Task<object> ScrapeAndDetectUrlAsync(string url, CancellationToken cancellationToken = default)
        {
            ValidateUrl(url, out var uri);
            var resolved = await DetermineAndResolveDirectUrlAsync(url, uri, depth: 0, cancellationToken);

            if (resolved.IsVideo)
            {
                return await ProcessVideoDirectUrlAsync(url, resolved.DirectUrl, resolved.DirectUri, cancellationToken);
            }
            else
            {
                return await ProcessTextDirectUrlAsync(url, resolved.DirectUrl, resolved.DirectUri, cancellationToken);
            }
        }

        #endregion

        #region Explicit Content Type Endpoints

        public async Task<ScrapedTextResponseDto> ScrapeTextUrlAsync(string url, CancellationToken cancellationToken = default)
        {
            ValidateUrl(url, out var uri);
            var resolved = await DetermineAndResolveDirectUrlAsync(url, uri, depth: 0, cancellationToken);

            if (resolved.IsVideo)
            {
                throw new AppException("The provided URL contains video content, but text was expected.", 415);
            }

            return await ProcessTextDirectUrlAsync(url, resolved.DirectUrl, resolved.DirectUri, cancellationToken);
        }

        public async Task<ScrapedVideoResponseDto> ScrapeVideoUrlAsync(string url, CancellationToken cancellationToken = default)
        {
            ValidateUrl(url, out var uri);
            var resolved = await DetermineAndResolveDirectUrlAsync(url, uri, depth: 0, cancellationToken);

            if (!resolved.IsVideo)
            {
                throw new AppException("The provided URL contains text content, but video was expected.", 415);
            }

            return await ProcessVideoDirectUrlAsync(url, resolved.DirectUrl, resolved.DirectUri, cancellationToken);
        }

        #endregion

        #region Infrastructure & Core Engines

        private void ValidateUrl(string url, out Uri uri)
        {
            if (string.IsNullOrWhiteSpace(url) || !Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                throw new AppException("The provided URL is invalid or malformed.", 400);
            }
        }

        private async Task<(string DirectUrl, Uri DirectUri, bool IsVideo)> DetermineAndResolveDirectUrlAsync(
            string url, Uri uri, int depth, CancellationToken cancellationToken)
        {
            if (depth > MaxVideoRedirectDepth)
            {
                throw new AppException("Could not resolve a direct resource due to too many redirects.", 400);
            }

            if (HasVideoExtension(uri)) return (uri.AbsoluteUri, uri, true);
            if (IsPlainTextField(uri)) return (uri.AbsoluteUri, uri, false);

            var client = _httpClientFactory.CreateClient("ContentScraper");
            using var request = new HttpRequestMessage(HttpMethod.Get, uri);

            // ???? ??????? ?????? ????? ????? ???? ?? ????? ???? ????? ???? ??? ??????
            request.Headers.TryAddWithoutValidation("User-Agent", BrowserUserAgent);
            request.Headers.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,video/*;q=0.8");

            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new AppException("The target resource could not be found.", 404);
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new AppException($"Remote platform rejected the request with status code: {(int)response.StatusCode}.", (int)response.StatusCode);
            }

            var mediaType = response.Content.Headers.ContentType?.MediaType?.ToLowerInvariant();
            if (mediaType != null && (mediaType.StartsWith("video/") || mediaType.StartsWith("application/x-mpegurl")))
            {
                return (uri.AbsoluteUri, uri, true);
            }

            return (uri.AbsoluteUri, uri, false);
        }

        private async Task<ScrapedVideoResponseDto> ProcessVideoDirectUrlAsync(string originalUrl, string directUrl, Uri uri, CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient("VideoScraper");
            using var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.TryAddWithoutValidation("User-Agent", BrowserUserAgent);

            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new AppException($"Failed to stream video binary. Status: {(int)response.StatusCode}.", 502);
            }

            var contentLength = response.Content.Headers.ContentLength ?? 0;
            if (contentLength > MaxVideoBytes)
            {
                throw new AppException("The video file size exceeds the allowed system limit.", 413);
            }

            var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            ValidateVideoBinary(bytes, directUrl);

            return new ScrapedVideoResponseDto
            {
                SourceUrl = originalUrl,
                DirectUrl = directUrl,
                VideoBinary = new VideoBinaryDto
                {
                    Data = bytes,
                    FileName = GetFileNameFromUrl(directUrl),
                    ContentType = response.Content.Headers.ContentType?.MediaType ?? "video/mp4"
                }
            };
        }

        private async Task<ScrapedTextResponseDto> ProcessTextDirectUrlAsync(string originalUrl, string directUrl, Uri uri, CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient("TextScraper");
            using var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.TryAddWithoutValidation("User-Agent", BrowserUserAgent);

            using var response = await client.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new AppException($"Failed to retrieve document content. Status: {(int)response.StatusCode}.", 502);
            }

            var htmlContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            var cleanText = HtmlAgilityPack.HtmlEntity.DeEntitize(htmlDoc.DocumentNode.InnerText?.Trim() ?? string.Empty);

            return new ScrapedTextResponseDto
            {
                SourceUrl = originalUrl,
                DirectUrl = directUrl,
                TextContent = cleanText
            };
        }

        private bool HasVideoExtension(Uri uri)
        {
            var ext = Path.GetExtension(uri.AbsolutePath).ToLowerInvariant();
            return ext == ".mp4" || ext == ".mkv" || ext == ".webm" || ext == ".avi" || ext == ".mov" || ext == ".m3u8";
        }

        private bool IsPlainTextField(Uri uri)
        {
            var ext = Path.GetExtension(uri.AbsolutePath).ToLowerInvariant();
            return ext == ".txt" || ext == ".json" || ext == ".xml";
        }

        private void ValidateVideoBinary(byte[] bytes, string url)
        {
            if (bytes == null || bytes.Length == 0)
            {
                throw new AppException("Downloaded video binary stream is empty.", 422);
            }
            if (!LooksLikeVideoBinary(bytes) && !HasVideoExtension(new Uri(url)))
            {
                throw new AppException("Content validation failed. Downloaded stream does not match video signature.", 422);
            }
        }

        private bool LooksLikeVideoBinary(byte[] bytes)
        {
            if (bytes.Length < 12) return false;
            if (bytes[4] == (byte)'f' && bytes[5] == (byte)'t' && bytes[6] == (byte)'y' && bytes[7] == (byte)'p') return true;
            if (bytes[0] == 0x1A && bytes[1] == 0x45 && bytes[2] == 0xDF && bytes[3] == 0xA3) return true;
            if (bytes[0] == (byte)'R' && bytes[1] == (byte)'I' && bytes[2] == (byte)'F' && bytes[3] == (byte)'F') return true;
            return false;
        }

        private string GetFileNameFromUrl(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return "video.mp4";
            var name = Path.GetFileName(uri.AbsolutePath);
            return string.IsNullOrWhiteSpace(name) ? "video.mp4" : name;
        }

        #endregion
    }
}
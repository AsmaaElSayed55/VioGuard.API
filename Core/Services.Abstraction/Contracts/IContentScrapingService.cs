using System.Threading;
using System.Threading.Tasks;
using Shared.Dtos.Content;

namespace Services.Abstraction.Contracts
{
    public interface IContentScrapingService
    {
        Task<object> ScrapeAndDetectUrlAsync(string url, CancellationToken cancellationToken = default);

        Task<ScrapedTextResponseDto> ScrapeTextUrlAsync(string url, CancellationToken cancellationToken = default);
        Task<ScrapedVideoResponseDto> ScrapeVideoUrlAsync(string url, CancellationToken cancellationToken = default);
    }
}
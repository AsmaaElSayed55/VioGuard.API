namespace Shared.Dtos.Content
{
    public class ScrapedContentResponseDto
    {
        public string SourceUrl { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string? TextContent { get; set; }
        public VideoBinaryDto? VideoBinary { get; set; }
    }
}

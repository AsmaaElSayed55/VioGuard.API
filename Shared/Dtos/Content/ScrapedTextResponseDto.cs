namespace Shared.Dtos.Content
{
    public class ScrapedTextResponseDto
    {
        public string SourceUrl { get; set; } = string.Empty;
        public string DirectUrl { get; set; } = string.Empty;
        public string ContentType { get; init; } = "Text";
        public string TextContent { get; set; } = string.Empty;
    }
}

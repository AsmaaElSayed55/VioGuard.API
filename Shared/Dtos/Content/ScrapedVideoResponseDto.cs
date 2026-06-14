namespace Shared.Dtos.Content
{
    public class ScrapedVideoResponseDto
    {
        public string SourceUrl { get; set; } = string.Empty;
        public string DirectUrl { get; set; } = string.Empty;
        public string ContentType { get; init; } = "Video";
        public VideoBinaryDto VideoBinary { get; set; } = new();
    }
}

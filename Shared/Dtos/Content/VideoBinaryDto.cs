namespace Shared.Dtos.Content
{
    public record VideoBinaryDto
    {
        public string FileName { get; init; } = string.Empty;
        public string ContentType { get; init; } = string.Empty;
        public long Length { get; init; }
        public byte[] Data { get; init; } = Array.Empty<byte>();
    }
}

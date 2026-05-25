namespace Shared.Dtos
{
    public record HistoryResultDto
    {
        public int Id { get; init; }
        public string ContentUrl { get; init; }
        public string ContentType { get; init; }
        public DateTime ActionDate { get; init; }
        public string AttachedUserEmail { get; init; }
    }
}

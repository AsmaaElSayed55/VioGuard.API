namespace Domain.Entities.SystemModule
{
    public class History : BaseEntity<int>
    {
        public int SystemId { get; set; }
        public SystemRoot System { get; set; } = null!;

        // Stores audit tracking for what was processed
        public string ContentUrl { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty; // "Video" or "Text"
        public DateTime ActionDate { get; set; } = DateTime.UtcNow;
        public string AttachedUserEmail { get; set; }
    }
}

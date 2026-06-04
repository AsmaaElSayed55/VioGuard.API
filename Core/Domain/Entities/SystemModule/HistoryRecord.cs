namespace Domain.Entities.SystemModule
{
    public class HistoryRecord : BaseEntity<string>
    {
        public string Id { get; set; } = string.Empty; // PK
        public string SystemId { get; set; } = string.Empty; // FK
        public string ContentUrl { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public DateTime ActionDate { get; set; }
        public string AttachedUserEmail { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModified { get; set; }

        // Navigation property
        public SystemRoot? SystemRoot { get; set; }
    }
}

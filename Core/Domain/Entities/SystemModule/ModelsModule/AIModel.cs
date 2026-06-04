namespace Domain.Entities.SystemModule.ModelsModule
{
    public class AIModel : BaseEntity<string>
    {
        public string Id { get; set; } = string.Empty; // PK
        public string Name { get; set; } = string.Empty;
        public string SystemId { get; set; } = string.Empty; // FK
        public string ModelType { get; set; } = string.Empty;
        public string Framework { get; set; } = string.Empty;
        public double AccuracyThreshold { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModified { get; set; }

        // Navigation property
        public SystemRoot? SystemRoot { get; set; }
    }
}

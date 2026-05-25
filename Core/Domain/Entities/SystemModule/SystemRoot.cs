namespace Domain.Entities.SystemModule
{
    public class SystemRoot : BaseEntity<string>
    {
        public string Id { get; set; } = string.Empty; // PK
        public string SystemName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModified { get; set; }

        // Navigation properties
        public ICollection<AIModel> AIModels { get; set; } = new List<AIModel>();
        public ICollection<HistoryRecord> Histories { get; set; } = new List<HistoryRecord>();
    }
}

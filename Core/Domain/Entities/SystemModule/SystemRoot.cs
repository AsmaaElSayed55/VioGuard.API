namespace Domain.Entities.SystemModule
{
    public class SystemRoot : BaseEntity<int>
    {
        public string SystemName { get; set; } = "VioGuard Engine";

        // Navigation Properties
        public ICollection<History> Histories { get; set; } = new List<History>();
        public ICollection<AIModel> Models { get; set; } = new List<AIModel>();
    }
}

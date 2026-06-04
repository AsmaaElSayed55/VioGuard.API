namespace Domain.Entities
{
    public abstract class BaseEntity<TKey>
    {
        // This will represent the primary key for every entity
        public TKey Id { get; set; } = default!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTime? LastModified { get; set; }
    }
}

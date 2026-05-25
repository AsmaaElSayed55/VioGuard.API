using System;
using Domain.Entities.UserModule;

namespace Domain.Entities.ContentsMudule
{
    public abstract class Content : BaseEntity<string>
    {
        // Your Primary Key (URL Column) mapped in configuration
        public DateTime DetectionDate { get; set; }
        public string UserEmail { get; set; } = string.Empty; // FK
        public string ContentType { get; set; } = string.Empty;

        // Shared auditing metadata properties
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModified { get; set; }

        // Navigation property
        public User? User { get; set; }
    }
}
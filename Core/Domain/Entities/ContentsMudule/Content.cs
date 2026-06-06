using System;
using Domain.Entities.UserModule;

namespace Domain.Entities.ContentsMudule
{
    public abstract class Content : BaseEntity<string>
    {
        public string URL { get; set; } = string.Empty;
        public DateTime DetectionDate { get; set; }
        public string UserEmail { get; set; } = string.Empty; // FK
        public string ContentType { get; set; } = string.Empty;

        // Navigation property
        public User? User { get; set; }
    }
}
using Domain.Entities.UserModule;
using System;
using System.Collections.Generic;
namespace Domain.Entities.ContentsMudule
{
    public abstract class Content : BaseEntity<int>
    {
        public string URL { get; set; }
        //public string Type { get; set; } // "Video" or "Text"
        public DateTime DetectionDate { get; set; }

        // Relationship with User
        public string UserEmail { get; set; }
        public User User { get; set; }
    }
}

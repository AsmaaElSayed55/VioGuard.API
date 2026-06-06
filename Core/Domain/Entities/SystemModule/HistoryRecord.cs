using Domain.Entities.UserModule;

namespace Domain.Entities.SystemModule
{
    public class HistoryRecord : BaseEntity<string>
    {
        public string ContentUrl { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public DateTime ActionDate { get; set; }
        public string AttachedUserEmail { get; set; } = string.Empty;

        public User? User { get; set; }
    }
}

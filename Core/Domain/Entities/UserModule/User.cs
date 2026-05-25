using Domain.Entities.ContentsMudule;

namespace Domain.Entities.UserModule
{
    // TKey is string because Email (Primary Key) is a string
    public class User : BaseEntity<string>
    {
        // The inherited 'Id' property will be mapped directly to the Email column
        public string FullName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        // This stores the internal alternate system string identifier shown in your diagram
        public string UserInternalId { get; set; } = string.Empty;
        public bool IsMonthlyReportEnabled { get; set; } = true;

        public ICollection<Content> Contents { get; set; } = new List<Content>();
    }
}
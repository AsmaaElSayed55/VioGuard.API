using Domain.Entities.ContentsMudule;
using Domain.Entities.SystemModule;
namespace Domain.Entities.UserModule
{
    public class User : BaseEntity<int>
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }

        // Navigation properties
        public ICollection<Content> UploadedContents { get; set; } = new List<Content>();
        public ICollection<Report> Reports { get; set; } = new List<Report>();
    }
}

namespace Domain.Entities.UserModule
{
    public class User : BaseEntity<int>
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }

        // Navigation properties
        public ICollection<Content> Contents { get; set; }
        public bool IsMonthlyReportEnabled { get; set; } = true;
    }
}

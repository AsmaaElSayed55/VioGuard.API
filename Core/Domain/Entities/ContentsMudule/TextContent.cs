namespace Domain.Entities.ContentsMudule
{
    public class TextContent : Content
    {
        public string textContext { get; set; } = string.Empty;
        public string ViolentResult { get; set; } = string.Empty;
        public string ViolentWords { get; set; } = string.Empty;
    }
}
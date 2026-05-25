namespace Domain.Entities.ContentsMudule
{
    public class TextContent : Content
    {
        public bool ViolentResult { get; set; }
        public string textContext { get; set; }
        public List<string> ViolentWords { get; set; }
    }
}

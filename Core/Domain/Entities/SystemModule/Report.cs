namespace Domain.Entities.SystemModule
{
    public class Report : BaseEntity<int>
    {
        public int NumOfVideo { get; private set; }
        public int NumOfText { get; private set; }
        public int ViolentText { get; private set; }
        public int ViolentVideo { get; private set; }

        // Constructor that dynamically builds the report metrics from user content
        public Report(IEnumerable<Content> contents)
        {
            var textContents = contents.OfType<TextContent>().ToList();
            var videoContents = contents.OfType<VideoContent>().ToList();

            NumOfText = textContents.Count;
            NumOfVideo = videoContents.Count;

            ViolentText = textContents.Count(t => t.ViolentResult);

            // Assuming a threshold (e.g., > 50%) marks a video as violent
            ViolentVideo = videoContents.Count(v => v.ViolentPercent > 50);
        }

        // Methods from your UML Diagram
        public double ViolentPercent()
        {
            int totalContent = TotalNumOfContent();
            if (totalContent == 0) return 0;

            return ((double)(ViolentText + ViolentVideo) / totalContent) * 100;
        }

        public int TotalNumOfContent()
        {
            return NumOfVideo + NumOfText;
        }

        public List<string> ShowReport()
        {
            return new List<string>
            {
                $"Total Texts Analyzed: {NumOfText} (Violent: {ViolentText})",
                $"Total Videos Analyzed: {NumOfVideo} (Violent: {ViolentVideo})",
                $"Overall Toxicity Percentage: {ViolentPercent():F2}%"
            };
        }
    }
}

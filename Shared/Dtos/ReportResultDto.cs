namespace Shared.Dtos
{
    public record ReportResultDto
    {
        public int TotalVideos { get; init; }
        public int TotalTexts { get; init; }
        public int ViolentTexts { get; init; }
        public int ViolentVideos { get; init; }
        public double TotalViolentPercent { get; init; }

    }
}

namespace Shared.Dtos.History
{
    public class HistoryDetailsDto
    {
        public string Id { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string FormattedDate { get; set; } = string.Empty; 
        public string FormattedTime { get; set; } = string.Empty; 
        public string CurrentStatus { get; set; } = string.Empty; 
        public string ConfidenceText { get; set; } = string.Empty; 

        public string? ExtractedTextContext { get; set; }

        public List<string> AnalysisSummary { get; set; } = new();
    }
}

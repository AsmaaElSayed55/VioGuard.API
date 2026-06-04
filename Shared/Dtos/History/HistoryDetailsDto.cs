namespace Shared.Dtos.History
{
    public record HistoryDetailsDto(
       string Id,
        DateTime ScannedAt,
        string ContentType,      // e.g., "Video Stream (MP4)" or "Text"
        bool IsVerified,
        string SourceUrl,
        string CurrentStatus,    // "Violent Content", "Against Violent Content", "Neutral Content"
        string StatusBadgeColor, // "Red", "Green", or "Blue" for easy frontend UI styling
        List<DetailFindingDto> AnalysisSummary
    );
}

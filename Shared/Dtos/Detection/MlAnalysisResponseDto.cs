namespace Shared.Dtos.Detection
{
    public record MlAnalysisResponseDto(
        string ContentType,
        bool ThreatFound,
        string ExtractedContext,
        List<MlRawFindingDto> RawFindings,
        byte[]? VideoBinary = null
    );
}

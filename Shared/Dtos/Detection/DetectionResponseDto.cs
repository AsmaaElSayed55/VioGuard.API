using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.Detection
{
    public record DetectionResponseDto(
        string Id,
        string SourceUrl,
        string ContentType,
        bool IsViolent,
        DateTime ProcessedAt,
        string StatusText,        // e.g., "Violent Content Detected"
        string ContextText,       // Code snippet text or video details
        List<FindingItemDto> Findings
    );
}        

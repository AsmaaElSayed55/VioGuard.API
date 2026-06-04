using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.AI_Models
{
    public record MlAnalysisResponseDto(
        string ContentType,       // "Text" or "Video"
        bool ThreatFound,
        string ExtractedContext,  // The raw text chunk or video metadata string
        List<MlRawFindingDto> RawFindings
    );
}
 
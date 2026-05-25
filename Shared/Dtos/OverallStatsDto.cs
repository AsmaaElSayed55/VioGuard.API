using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos
{
    public record OverallStatsDto(
        int TotalAnalyses,
        int ViolentIncidents,
        int NonViolentAnalyses,
        int AgainstViolenceAnalyses,
        int NeutralTextAnalyses
    );
}

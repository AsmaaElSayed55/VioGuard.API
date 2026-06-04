using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.Report
{
    public record AggregateMetricsDto(
        int TotalCount,
        int ViolentCount,
        int NonViolentCount,
        int AgainstViolenceCount, // Specific to your text-analysis screen rules
        int NeutralCount
    );

}

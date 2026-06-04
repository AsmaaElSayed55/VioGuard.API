using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.Report
{
    public record UpdateReportSettingsDto(
        bool EnableMonthlyReports
    );
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.Detection
{
    public record AnalyzeRequestDto(string Url, string? UserEmail = null);
}

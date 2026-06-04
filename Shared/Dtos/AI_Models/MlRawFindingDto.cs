using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.AI_Models
{
    public record MlRawFindingDto(string Label, string Description, bool IsViolation);
}

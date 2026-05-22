using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos
{
    public record CreateHistoryDto
    {
        public string ContentUrl { get; init; }
        public string ContentType { get; init; }
        public string UserEmail { get; init; }
    }
}

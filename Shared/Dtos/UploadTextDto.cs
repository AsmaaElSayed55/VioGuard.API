using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos
{
    public record UploadTextDto
    {
        public string Url { get; init; }    
        public string TextContext { get; init; }
        public string UserEmail { get; init; }
    }
}

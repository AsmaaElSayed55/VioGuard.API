using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.Content
{
    public class CreateTextContentDto
    {
        public string URL { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string TextContext { get; set; } = string.Empty;
        public List<string> ViolentWords { get; set; } = new List<string>();
    }
}

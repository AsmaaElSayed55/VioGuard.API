using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.Content
{
    public class TextContentDto : ContentDto
    {
        public string Url { get; set; } = string.Empty;
        public string textContext { get; set; } = string.Empty;
        public string ViolentResult { get; set; } = string.Empty;
        public List<string> ViolentWords { get; set; } = new List<string>();
    }
}

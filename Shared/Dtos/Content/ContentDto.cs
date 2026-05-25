using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.Content
{
    public class ContentDto
    {
        public string URL { get; set; } = string.Empty;
        public DateTime DetectionDate { get; set; }
        public string UserEmail { get; set; } = string.Empty;
    }
}

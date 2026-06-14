using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.Content
{
    public class VideoContentDto : ContentDto
    {
        public string Url { get; set; } = string.Empty;
        public double ViolentPercent { get; set; }
    }
}

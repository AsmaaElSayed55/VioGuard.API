using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.System
{
    public class AIModelDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ModelType { get; set; } = string.Empty;
        public string Framework { get; set; } = string.Empty;
        public double AccuracyThreshold { get; set; }
    }
}

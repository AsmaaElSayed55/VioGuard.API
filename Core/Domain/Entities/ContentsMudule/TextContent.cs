using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.ContentsMudule
{
    public class TextContent : Content
    {
        public bool ViolentResult { get; set; }
        public string textContext { get; set; }
        public List<string> ViolentWords { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.System
{
    public class HistoryRecordDto
    {
        public string Id { get; set; } = string.Empty;
        public string ContentUrl { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public DateTime ActionDate { get; set; }
        public string AttachedUserEmail { get; set; } = string.Empty;
    }
}

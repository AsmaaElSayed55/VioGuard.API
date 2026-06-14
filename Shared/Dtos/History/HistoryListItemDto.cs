using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.History
{
    public record HistoryListItemDto
    {
        public string Id { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty; // "Text" أو "Video"
        public string TimeAgo { get; set; } = string.Empty; // مثل "2 hours ago" أو "Yesterday"
        public string Status { get; set; } = string.Empty; // "Safe" أو "Flagged"
        public DateTime DetectionDate { get; set; }
    }
}

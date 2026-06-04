using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos.History
{
    public record HistoryListItemDto(
        string Id,
        string DomainName,       // e.g., "youtube.com/watch?v=dQw..."
        string ContentType,     // "Text" or "Video"
        string RelativeTime,    // e.g., "2 hours ago", "Yesterday", "28/04/2024"
        string SafetyStatus     // "Safe" or "Flagged"
    );
}

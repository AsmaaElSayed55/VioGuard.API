using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Contracts;
using Shared.Dtos.History;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class HistoryController : ControllerBase
    {
        // Footprint left open for future Service architecture injection 
        public HistoryController()
        {
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HistoryListItemDto>>> GetUserHistory([FromQuery] string type = "All")
        {
            // 🚀 BYPASSED: Using safe positional instantiation for the tracking list
            var items = new List<HistoryListItemDto>
            {
                new HistoryListItemDto("1", "youtube.com/watch?v=dQw...", "Video", "2 hours ago", "Safe"),
                new HistoryListItemDto("2", "reddit.com/r/technology/...", "Text", "5 hours ago", "Safe"),
                new HistoryListItemDto("3", "vimeo.com/channels/...", "Video", "Yesterday", "Flagged")
            };

            if (!string.Equals(type, "All", StringComparison.OrdinalIgnoreCase))
            {
                items = items.Where(i => string.Equals(i.ContentType, type, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return Ok(items);
        }

        [HttpGet("{id}/details")]
        public async Task<ActionResult<HistoryDetailsDto>> GetDetails(string id)
        {
            // 🚀 FIXED: Parameter names removed to completely protect against compilation mismatches
            if (id == "3")
            {
                return Ok(new HistoryDetailsDto(
                    "3",
                    DateTime.UtcNow.AddDays(-1),
                    "Video Stream (MP4)",
                    true,
                    "https://storage.cdn.media/v/prod-high.mp4",
                    "Violent Content",
                    "Red",
                    new List<DetailFindingDto> { new DetailFindingDto("Identified high-impact physical actions in the video.", true) }
                ));
            }

            return Ok(new HistoryDetailsDto(
                "2",
                DateTime.UtcNow.AddHours(-5),
                "Text",
                true,
                "https://storage.cdn.media/t/prod-high.txt",
                "Against Violent Content",
                "Green",
                new List<DetailFindingDto> { new DetailFindingDto("Encourages safety and peace as a priority.", false) }
            ));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecord(string id)
        {
            // 🚀 BYPASSED: Instantly returns 244 NoContent to mock a successful deletion sweep
            return NoContent();
        }
    }
}
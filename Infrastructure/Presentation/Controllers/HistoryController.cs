using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Contracts;
using Shared.Dtos.History;
namespace Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class HistoryController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HistoryListItemDto>>> GetUserHistory([FromQuery] string type = "All")
        {
            // Direct query layout replacing raw values: _context.Histories.Where(...)
            var items = new List<HistoryListItemDto>
      {
        new("1", "youtube.com/watch?v=dQw...", "Video", "2 hours ago", "Safe"),
        new("2", "reddit.com/r/technology/...", "Text", "5 hours ago", "Safe"),
        new("3", "vimeo.com/channels/...", "Video", "Yesterday", "Flagged")
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
            // Direct query mapping layout from DB records: _context.Histories.Find(id)
            if (id == "3")
            {
                return Ok(new HistoryDetailsDto(
                  Id: "3", ScannedAt: DateTime.UtcNow.AddDays(-1), ContentType: "Video Stream (MP4)", IsVerified: true,
                  SourceUrl: "https://storage.cdn.media/v/prod-high.mp4", CurrentStatus: "Violent Content", StatusBadgeColor: "Red",
                  AnalysisSummary: new List<DetailFindingDto> { new("Identified high-impact physical actions in the video.", true) }
                ));
            }

            return Ok(new HistoryDetailsDto(
              Id: "2", ScannedAt: DateTime.UtcNow.AddHours(-5), ContentType: "Text", IsVerified: true,
              SourceUrl: "https://storage.cdn.media/t/prod-high.txt", CurrentStatus: "Against Violent Content", StatusBadgeColor: "Green",
              AnalysisSummary: new List<DetailFindingDto> { new("Encourages safety and peace as a priority.", false) }
            ));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecord(string id) => NoContent();
    }
}

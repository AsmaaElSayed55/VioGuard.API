using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
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
        private readonly IHistoryService _historyService;

        public HistoryController(IHistoryService historyService)
        {
            _historyService = historyService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HistoryListItemDto>>> GetUserHistory(
            [FromQuery] string type = "All",
            [FromQuery] string? userEmail = null)
        {
            var email = ResolveUserEmail(userEmail);
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest(new { Message = "User email is required." });

            var items = await _historyService.GetUserHistoryAsync(email, type);
            return Ok(items);
        }

        [HttpGet("{id}/details")]
        public async Task<ActionResult<HistoryDetailsDto>> GetDetails(string id)
        {
            var details = await _historyService.GetDetailsAsync(id);
            if (details is null)
                return NotFound(new { Message = "History record not found." });

            return Ok(details);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecord(string id)
        {
            var deleted = await _historyService.DeleteRecordAsync(id);
            if (!deleted)
                return NotFound(new { Message = "History record not found." });

            return NoContent();
        }

        private string? ResolveUserEmail(string? queryEmail)
        {
            if (!string.IsNullOrWhiteSpace(queryEmail))
                return queryEmail;

            return User.FindFirstValue(ClaimTypes.Email);
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Contracts;
using Shared.Dtos;
namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HistoryController : ControllerBase
    {
        private readonly IHistoryService _historyService;

        public HistoryController(IHistoryService historyService)
        {
            _historyService = historyService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetGlobalLogs()
        {
            var logs = await _historyService.GetAllHistoriesAsync();
            return Ok(logs);
        }

        [HttpGet("user/{email}")]
        public async Task<IActionResult> GetUserLogs(string email)
        {
            var userLogs = await _historyService.GetUserHistoriesAsync(email);
            return Ok(userLogs);
        }

        [HttpPost("log")]
        public async Task<IActionResult> CreateLog([FromBody] CreateHistoryDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _historyService.CreateLogAsync(dto);
            return Ok(new { status = "Success", message = "System audit footprint generated." });
        }
    }
}

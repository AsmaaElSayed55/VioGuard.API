using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Contracts;
using Shared.Dtos.Content;

namespace Presentation.Controllers
{
    [Authorize] 
    [ApiController]
    [Route("api/v1/content")]
    public class ContentController : ControllerBase
    {
        private readonly IContentScrapingService _scrapingService;

        public ContentController(IContentScrapingService scrapingService)
        {
            _scrapingService = scrapingService;
        }


        [HttpPost("scrape-any")]
        public async Task<IActionResult> ScrapeAnyUrl([FromBody] string url, CancellationToken cancellationToken)
        {
            var result = await _scrapingService.ScrapeAndDetectUrlAsync(url, cancellationToken);
            return Ok(result);
        }

        [HttpPost("scrape-text")]
        public async Task<ActionResult<ScrapedTextResponseDto>> ScrapeTextUrl([FromBody] string url, CancellationToken cancellationToken)
        {
            var result = await _scrapingService.ScrapeTextUrlAsync(url, cancellationToken);
            return Ok(result);
        }

        [HttpPost("scrape-video")]
        public async Task<ActionResult<ScrapedVideoResponseDto>> ScrapeVideoUrl([FromBody] string url, CancellationToken cancellationToken)
        {
            var result = await _scrapingService.ScrapeVideoUrlAsync(url, cancellationToken);
            return Ok(result);
        }
    }
}
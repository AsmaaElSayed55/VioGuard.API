using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.AI_Models;
using Shared.Dtos.Detection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DetectionController : ControllerBase
    {
        private readonly HttpClient _mlHttpClient;

        // Kept intact to protect against Dependency Injection container errors on startup
        public DetectionController(HttpClient mlHttpClient)
        {
            _mlHttpClient = mlHttpClient;
        }

        [HttpPost("analyze")]
        public async Task<ActionResult<DetectionResponseDto>> AnalyzeContent([FromBody] AnalyzeRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Url))
                return BadRequest("Invalid target destination link.");

            // 🚀 BYPASSED: Creating static findings mock list
            var staticFindings = new List<FindingItemDto>
            {
                new FindingItemDto("Profanity", "Detected high-frequency inappropriate linguistic markers.", true),
                new FindingItemDto("Aggressive Intent", "Text semantics express structural hostility.", true),
                new FindingItemDto("Graphic Visuals", "Metadata scanners flagged matching visual frames.", false)
            };

            // 🚀 BYPASSED: Mapping straight to the UI Response DTO using safe positional parameters
            var finalUiResult = new DetectionResponseDto(
                Guid.NewGuid().ToString()[..6],                          // Id
                request.Url,                                              // SourceUrl
                "Text/Video Mixture",                                     // ContentType
                true,                                                     // IsViolent
                DateTime.UtcNow,                                          // ProcessedAt
                "Violent Content Detected",                               // StatusText
                "Flagged phrases: 'assault', 'weapon', and tactical terms.", // ContextText
                staticFindings                                            // Findings List
            );

            // Bypassing DB saving tasks during manual frontend API test sweeps
            return Ok(finalUiResult);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared.Dtos.Detection;
using Shared.Dtos.Content;
using Domain.Entities.ContentsMudule;
using Services.Abstraction.Contracts;

namespace Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DetectionController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<DetectionController> _logger;
        private readonly IApplicationDbContext _context;

        public DetectionController(
            IHttpClientFactory httpClientFactory,
            ILogger<DetectionController> logger,
            IApplicationDbContext context)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _context = context;
        }

        [HttpPost("analyze")]
        public async Task<ActionResult<DetectionResponseDto>> AnalyzeContent([FromBody] AnalyzeRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Url))
                return BadRequest("Invalid target destination link.");

            try
            {
                var mlClient = _httpClientFactory.CreateClient("MlService");
                var mlRequestPayload = new MlAnalysisRequestDto(request.Url);
                var bodyContent = new StringContent(
                    JsonSerializer.Serialize(mlRequestPayload),
                    System.Text.Encoding.UTF8,
                    "application/json");

                var response = await mlClient.PostAsync("api/v1/predict", bodyContent);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var mlResult = JsonSerializer.Deserialize<MlAnalysisResponseDto>(
                    jsonString,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (mlResult is null)
                    return StatusCode(502, "ML service returned an empty response.");

                var dynamicFindings = mlResult.RawFindings
                    .Select(f => new FindingItemDto(f.Label, f.Description, f.IsViolation))
                    .ToList();

                var finalUiResult = new DetectionResponseDto(
                    Id: Guid.NewGuid().ToString()[..8],
                    SourceUrl: request.Url,
                    ContentType: mlResult.ContentType,
                    IsViolent: mlResult.ThreatFound,
                    ProcessedAt: DateTime.UtcNow,
                    StatusText: mlResult.ThreatFound ? "Violent Content Detected" : "Non-Violent Content Detected",
                    ContextText: mlResult.ExtractedContext,
                    Findings: dynamicFindings
                );

                return Ok(finalUiResult);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(ex, "ML service unavailable for URL {Url}", request.Url);
                return StatusCode(503, new { Message = "ML detection service is unavailable. Please try again later." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Detection failed for URL {Url}", request.Url);
                return StatusCode(500, new { Message = "Content analysis failed." });
            }
        }

        [HttpPost("save-text-result")]
        public async Task<IActionResult> SaveTextResult([FromBody] TextContentDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Url))
                return BadRequest("Invalid text content data payload.");

            try
            {
                var currentUserEmail = ResolveUserEmail();
                if (string.IsNullOrWhiteSpace(currentUserEmail))
                    return Unauthorized("Could not identify the logged-in user from token context.");

                var textEntity = new TextContent
                {
                    Id = Guid.NewGuid().ToString()[..8],
                    URL = dto.Url,
                    DetectionDate = DateTime.UtcNow,
                    UserEmail = currentUserEmail,
                    ContentType = "Text",
                    textContext = dto.textContext,
                    ViolentResult = dto.ViolentResult,
                    ViolentWords = dto.ViolentWords != null ? string.Join(", ", dto.ViolentWords) : string.Empty
                };

                await _context.TextContents.AddAsync(textEntity);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Text analysis logged and saved successfully.", RecordId = textEntity.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to persist text detection record for URL: {Url}", dto.Url);
                return StatusCode(500, new { Message = "Internal database persistence error while saving text content." });
            }
        }

        [HttpPost("save-video-result")]
        public async Task<IActionResult> SaveVideoResult([FromBody] VideoContentDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Url))
                return BadRequest("Invalid video content data payload.");

            try
            {
                var currentUserEmail = ResolveUserEmail();
                if (string.IsNullOrWhiteSpace(currentUserEmail))
                    return Unauthorized("Could not identify the logged-in user from token context.");

                var videoEntity = new VideoContent
                {
                    Id = Guid.NewGuid().ToString()[..8],
                    URL = dto.Url,
                    DetectionDate = DateTime.UtcNow,
                    UserEmail = currentUserEmail,
                    ContentType = "Video",
                    ViolentPercent = dto.ViolentPercent
                };

                await _context.VideoContents.AddAsync(videoEntity);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Video analysis logged and saved successfully.", RecordId = videoEntity.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to persist video detection record for URL: {Url}", dto.Url);
                return StatusCode(500, new { Message = "Internal database persistence error while saving video content." });
            }
        }

        private string? ResolveUserEmail()
        {
            return User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirst("email")?.Value;
        }
    }
}
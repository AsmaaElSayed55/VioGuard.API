using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared.Dtos.Detection;
using System.Text.Json;

namespace Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/[controller]")]
    public class DetectionController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<DetectionController> _logger;

        public DetectionController(IHttpClientFactory httpClientFactory, ILogger<DetectionController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
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
    }
}

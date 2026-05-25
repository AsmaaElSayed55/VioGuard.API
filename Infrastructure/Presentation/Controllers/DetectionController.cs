using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Dtos.AI_Models;
using Shared.Dtos.Detection;
using System.Text.Json;
namespace Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DetectionController : ControllerBase
    {
        private readonly HttpClient _mlHttpClient;

        public DetectionController(HttpClient mlHttpClient)
        {
            _mlHttpClient = mlHttpClient;
            // Assumes client base address is safely registered to Python FastAPI server in Program.cs
        }

        [HttpPost("analyze")]
        public async Task<ActionResult<DetectionResponseDto>> AnalyzeContent([FromBody] AnalyzeRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Url)) return BadRequest("Invalid target destination link.");

            // 1. Pack outbound payload for Python ML API microservice
            var mlRequestPayload = new MlAnalysisRequestDto(request.Url);
            var bodyContent = new StringContent(JsonSerializer.Serialize(mlRequestPayload), System.Text.Encoding.UTF8, "application/json");

            // 2. Transmit across HTTP Pipeline to Python model endpoint
            var response = await _mlHttpClient.PostAsync("api/v1/predict", bodyContent);
            response.EnsureSuccessStatusCode();

            // 3. Receive the evaluated mathematical/linguistic payload back
            var jsonString = await response.Content.ReadAsStringAsync();
            var mlResult = JsonSerializer.Deserialize<MlAnalysisResponseDto>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // 4. Transform native ML model labels back into your specific Mobile UI schema mappings
            var dynamicFindings = mlResult.RawFindings.Select(f => new FindingItemDto(f.Label, f.Description, f.IsViolation)).ToList();

            var finalUiResult = new DetectionResponseDto(
                Id: Guid.NewGuid().ToString()[..6],
                SourceUrl: request.Url,
                ContentType: mlResult.ContentType,
                IsViolent: mlResult.ThreatFound,
                ProcessedAt: DateTime.UtcNow,
                StatusText: mlResult.ThreatFound ? "Violent Content Detected" : "Non-Violent Content Detected",
                ContextText: mlResult.ExtractedContext,
                Findings: dynamicFindings
            );

            // 5. TODO: Save finalUiResult down into DbContext Entities via EF Core before outputting
            // _context.Histories.Add(entity); await _context.SaveChangesAsync();

            return Ok(finalUiResult);
        }
    }
}

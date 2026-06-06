using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using Services.Abstraction.Contracts;
using Shared.Dtos.Content;

namespace Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ContentController : ControllerBase
    {
        private readonly IContentService _contentService;

        public ContentController(IContentService contentService)
        {
            _contentService = contentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContentDto>>> GetUserContents([FromQuery] string userEmail)
        {
            if (string.IsNullOrWhiteSpace(userEmail))
                return BadRequest(new { Message = "User email is required." });

            var contents = await _contentService.GetUserContentsAsync(userEmail);
            return Ok(contents);
        }

        [HttpPost("text")]
        public async Task<ActionResult<TextContentDto>> SaveTextContent([FromBody] CreateTextContentDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.URL) || string.IsNullOrWhiteSpace(dto.UserEmail))
                return BadRequest(new { Message = "URL and user email are required." });

            try
            {
                var result = await _contentService.AddTextContentAsync(dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
        }

        [HttpPost("video")]
        public async Task<ActionResult<VideoContentDto>> SaveVideoContent([FromBody] CreateVideoContentDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.URL) || string.IsNullOrWhiteSpace(dto.UserEmail))
                return BadRequest(new { Message = "URL and user email are required." });

            try
            {
                var result = await _contentService.AddVideoContentAsync(dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
        }

        [HttpPost("upload-video")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(VideoBinaryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<VideoBinaryDto>> UploadVideo([FromForm] UploadVideoForm form)
        {
            var video = form.Video;
            if (video is null || video.Length == 0)
                return BadRequest("A video file is required.");

            try
            {
                using var memoryStream = new MemoryStream();
                await video.CopyToAsync(memoryStream);
                var binaryData = await _contentService.ConvertVideoFileToBinaryAsync(memoryStream.ToArray());
                return Ok(new VideoBinaryDto
                {
                    FileName = video.FileName,
                    ContentType = video.ContentType,
                    Length = binaryData.LongLength,
                    Data = binaryData
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

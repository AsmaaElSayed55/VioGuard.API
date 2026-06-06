using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("upload-video")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(VideoBinaryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<VideoBinaryDto>> UploadVideo([FromForm] IFormFile video)
        {
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

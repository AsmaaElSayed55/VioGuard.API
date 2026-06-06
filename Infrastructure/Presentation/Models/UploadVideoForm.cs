using Microsoft.AspNetCore.Http;

namespace Presentation.Models
{
    public class UploadVideoForm
    {
        public IFormFile Video { get; set; } = null!;
    }
}

using Shared.Dtos.AI_Models;
using Shared.Dtos.Content;
using Shared.Dtos.History;
using Shared.Dtos.Report;
namespace Services.Abstraction.Contracts
{
    public interface IContentService
    {
        Task<IEnumerable<ContentDto>> GetUserContentsAsync(string userEmail);
        Task<TextContentDto> AddTextContentAsync(CreateTextContentDto textDto);
        Task<VideoContentDto> AddVideoContentAsync(CreateVideoContentDto videoDto);
        // In IContentService.cs
        Task<byte[]> ConvertVideoFileToBinaryAsync(byte[] videoBytes);
    }
}

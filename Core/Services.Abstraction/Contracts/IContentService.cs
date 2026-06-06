using Shared.Dtos.Content;

namespace Services.Abstraction.Contracts
{
    public interface IContentService
    {
        Task<IEnumerable<ContentDto>> GetUserContentsAsync(string userEmail);
        Task<TextContentDto> AddTextContentAsync(CreateTextContentDto textDto);
        Task<VideoContentDto> AddVideoContentAsync(CreateVideoContentDto videoDto);
        Task<byte[]> ConvertVideoFileToBinaryAsync(byte[] videoBytes);
    }
}

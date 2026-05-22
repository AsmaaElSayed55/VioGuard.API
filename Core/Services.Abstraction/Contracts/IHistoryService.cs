using Shared.Dtos;
namespace Services.Abstraction.Contracts
{
    public interface IHistoryService
    {
        Task<IEnumerable<HistoryResultDto>> GetAllHistoriesAsync();
        Task<IEnumerable<HistoryResultDto>> GetUserHistoriesAsync(string email);
        Task CreateLogAsync(CreateHistoryDto dto);
    }
}

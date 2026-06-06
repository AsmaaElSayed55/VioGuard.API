using Shared.Dtos.History;

namespace Services.Abstraction.Contracts
{
    public interface IHistoryService
    {
        Task<IEnumerable<HistoryListItemDto>> GetUserHistoryAsync(string userEmail, string typeFilter = "All");
        Task<HistoryDetailsDto?> GetDetailsAsync(string id);
        Task<bool> DeleteRecordAsync(string id);
    }
}

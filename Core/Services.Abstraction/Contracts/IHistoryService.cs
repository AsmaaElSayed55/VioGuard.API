using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Dtos.History;

namespace Services.Abstraction.Contracts
{
    public interface IHistoryService
    {
        Task<IEnumerable<HistoryListItemDto>> GetUserHistoryAsync(string userEmail, string type);
        Task<HistoryDetailsDto?> GetDetailsAsync(string id);
        Task<bool> DeleteRecordAsync(string id);
    }
}
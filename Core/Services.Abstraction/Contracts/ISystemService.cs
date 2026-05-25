using Shared.Dtos.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstraction.Contracts
{
    public interface ISystemService
    {
        Task<IEnumerable<AIModelDto>> GetModelsBySystemAsync(string systemId);
        Task<IEnumerable<HistoryRecordDto>> GetSystemAuditLogsAsync(string systemId);
    }
}

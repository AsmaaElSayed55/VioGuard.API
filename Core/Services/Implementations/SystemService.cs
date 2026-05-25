using AutoMapper;
using Domain.Contracts;
using Domain.Entities.ContentsMudule;
using Domain.Entities.SystemModule;
using Domain.Entities.SystemModule.ModelsModule;
using Domain.Entities.UserModule;
using Services.Abstraction.Contracts;
using Shared.Dtos;
using Shared.Dtos.Report;
using Shared.Dtos.System;
namespace Services.Implementations
{
    public class SystemService : ISystemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SystemService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AIModelDto>> GetModelsBySystemAsync(string systemId)
        {
            var repo = _unitOfWork.GetRepository<AIModel, string>();
            var models = await repo.GetAllAsync(asNoTracking: true);

            var filtered = models.Where(m => m.SystemId.Equals(systemId, StringComparison.OrdinalIgnoreCase));
            return _mapper.Map<IEnumerable<AIModelDto>>(filtered);
        }

        public async Task<IEnumerable<HistoryRecordDto>> GetSystemAuditLogsAsync(string systemId)
        {
            var repo = _unitOfWork.GetRepository<HistoryRecord, string>();
            var logs = await repo.GetAllAsync(asNoTracking: true);

            var filtered = logs.Where(l => l.SystemId.Equals(systemId, StringComparison.OrdinalIgnoreCase));
            return _mapper.Map<IEnumerable<HistoryRecordDto>>(filtered);
        }
    }
}
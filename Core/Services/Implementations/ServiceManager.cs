using AutoMapper;
using Domain.Contracts;
using Services.Abstraction.Contracts;
namespace Services.Implementations
{

    public class ServiceManager(IUnitOfWork _unitOfWork, IMapper _mapper) : IServiceManager
    {
        private readonly Lazy<IHistoryService> _historyService = new Lazy<IHistoryService>(() => new HistoryService(_unitOfWork, _mapper));

        public IHistoryService HistoryService => _historyService.Value;
    }
    
}

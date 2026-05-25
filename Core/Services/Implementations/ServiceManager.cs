using AutoMapper;
using Domain.Contracts;
using Services.Abstraction.Contracts;

namespace Services.Implementations
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IUserService> _userService;
        private readonly Lazy<IContentService> _contentService;
        private readonly Lazy<ISystemService> _systemService;
        private readonly Lazy<IReportService> _reportService;

        public ServiceManager(IUnitOfWork unitOfWork, IMapper mapper)
        {
            // Each service is lazily instantiated with its correct implementation type
            _userService = new Lazy<IUserService>(() => new UserService(unitOfWork, mapper));
            _contentService = new Lazy<IContentService>(() => new ContentService(unitOfWork, mapper));
            _systemService = new Lazy<ISystemService>(() => new SystemService(unitOfWork, mapper));
            _reportService = new Lazy<IReportService>(() => new ReportService(unitOfWork));
        }

        public IUserService UserService => _userService.Value;
        public IContentService ContentService => _contentService.Value;
        public ISystemService SystemService => _systemService.Value;
        public IReportService ReportService => _reportService.Value;
    }
}
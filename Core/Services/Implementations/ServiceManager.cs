using Services.Abstraction.Contracts;

namespace Services.Implementations
{
    public class ServiceManager : IServiceManager
    {
        public ServiceManager(
            IUserService userService,
            IContentService contentService,
            IHistoryService historyService,
            IReportService reportService)
        {
            UserService = userService;
            ContentService = contentService;
            HistoryService = historyService;
            ReportService = reportService;
        }

        public IUserService UserService { get; }
        public IContentService ContentService { get; }
        public IHistoryService HistoryService { get; }
        public IReportService ReportService { get; }
    }
}

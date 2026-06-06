namespace Services.Abstraction.Contracts
{
    public interface IServiceManager
    {
        IUserService UserService { get; }
        IContentService ContentService { get; }
        IHistoryService HistoryService { get; }
        IReportService ReportService { get; }
    }
}

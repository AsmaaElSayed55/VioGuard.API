namespace Services.Abstraction.Contracts
{
    public interface IServiceManager
    {
        IUserService UserService { get; }
        IContentService ContentService { get; }
        ISystemService SystemService { get; }
        IReportService ReportService { get; }
    }
}
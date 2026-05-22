using Shared.Dtos;
namespace Services.Abstraction.Contracts
{
    public interface IContentService
    {
        Task ProcessTextContentAsync(UploadTextDto dto);
        Task ProcessVideoContentAsync(UploadVideoDto dto);
        Task<ReportResultDto> GetUserReportAsync(string email);
        Task<IEnumerable<HistoryResultDto>> GetUserHistoryAsync(string email);
    }
}

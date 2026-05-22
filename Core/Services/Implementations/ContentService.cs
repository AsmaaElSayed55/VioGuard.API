using Domain.Contracts;
using Domain.Entities.ContentsMudule;
using Domain.Entities.SystemModule;
using Domain.Entities.UserModule;
using Services.Abstraction.Contracts;
using Shared.Dtos;

namespace Services.Implementations
{
    public class ContentService : IContentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ContentService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public Task<IEnumerable<HistoryResultDto>> GetUserHistoryAsync(string email)
        {
            throw new NotImplementedException();
        }

        // Dynamic, live calculation endpoint logic
        public async Task<Report> GetUserReportAsync(string email)
        {
            var userRepo = _unitOfWork.GetRepository<User, int>();
            var contentRepo = _unitOfWork.GetRepository<Content, int>();

            var users = await userRepo.GetAllAsync();
            var user = users.FirstOrDefault(u => u.Email == email);
            if (user == null) throw new Exception("User not found.");

            // Fetch all contents uploaded by this specific user
            var allContents = await contentRepo.GetAllAsync(asNoTracking: true);
            var userContents = allContents.Where(c => c.UserEmail == user.Email);

            // Dynamically instantiate the Report without database overhead
            return new Report(userContents);
        }

        public Task ProcessTextContentAsync(UploadTextDto dto)
        {
            throw new NotImplementedException();
        }

        public Task ProcessVideoContentAsync(UploadVideoDto dto)
        {
            throw new NotImplementedException();
        }

        Task<ReportResultDto> IContentService.GetUserReportAsync(string email)
        {
            throw new NotImplementedException();
        }
    }
}

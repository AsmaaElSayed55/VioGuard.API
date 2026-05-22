using Domain.Contracts;
using Domain.Entities.SystemModule;
using Services.Abstraction.Contracts;
using Shared.Dtos;
namespace Services.Implementations
{
    public class HistoryManager : IHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public HistoryManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<HistoryResultDto>> GetAllHistoriesAsync()
        {
            var historyRepo = _unitOfWork.GetRepository<History, int>();
            var histories = await historyRepo.GetAllAsync(asNoTracking: true);

            return histories.Select(h => new HistoryResultDto
            {
                Id = h.Id,
                ContentUrl = h.ContentUrl,
                ContentType = h.ContentType,
                ActionDate = h.ActionDate,
                AttachedUserEmail = h.AttachedUserEmail
            }).ToList();
        }

        public async Task<IEnumerable<HistoryResultDto>> GetUserHistoriesAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email parameter target is invalid.");

            var historyRepo = _unitOfWork.GetRepository<History, int>();
            var histories = await historyRepo.GetAllAsync(asNoTracking: true);

            return histories
                .Where(h => h.AttachedUserEmail.Equals(email, StringComparison.OrdinalIgnoreCase))
                .Select(h => new HistoryResultDto
                {
                    Id = h.Id,
                    ContentUrl = h.ContentUrl,
                    ContentType = h.ContentType,
                    ActionDate = h.ActionDate,
                    AttachedUserEmail =             h.AttachedUserEmail
                }).ToList();
        }

        public async Task CreateLogAsync(CreateHistoryDto dto)
        {
            var historyRepo = _unitOfWork.GetRepository<History, int>();
            var systemRepo = _unitOfWork.GetRepository<SystemRoot, int>();

            // Ensure a core System Engine root environment domain entry exists
            var systems = await systemRepo.GetAllAsync();
            var systemEngine = systems.FirstOrDefault();
            if (systemEngine == null)
            {
                systemEngine = new SystemRoot { SystemName = "VioGuard Central Engine" };
                await systemRepo.AddAsync(systemEngine);
                await _unitOfWork.SaveChangesAsync();
            }

            var newLog = new History
            {
                ContentUrl = dto.ContentUrl,
                ContentType = dto.ContentType,
                AttachedUserEmail = dto.UserEmail,
                ActionDate = DateTime.UtcNow,
                SystemId = systemEngine.Id
            };

            await historyRepo.AddAsync(newLog);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}

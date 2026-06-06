using AutoMapper;
using Domain.Contracts;
using Domain.Entities.ContentsMudule;
using Domain.Entities.SystemModule;
using Services.Abstraction.Contracts;
using Shared.Dtos.Content;

namespace Services.Implementations
{
    public class ContentService : IContentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ContentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ContentDto>> GetUserContentsAsync(string userEmail)
        {
            var repo = _unitOfWork.GetRepository<Content, string>();
            var allContents = await repo.GetAllAsync(asNoTracking: true);
            var filtered = allContents.Where(c => c.UserEmail.Equals(userEmail, StringComparison.OrdinalIgnoreCase));
            return _mapper.Map<IEnumerable<ContentDto>>(filtered);
        }

        public async Task<TextContentDto> AddTextContentAsync(CreateTextContentDto textDto)
        {
            var contentRepo = _unitOfWork.GetRepository<Content, string>();
            var existing = await contentRepo.GetByIdAsync(textDto.URL);
            if (existing != null)
                throw new InvalidOperationException("Content for this URL already exists.");

            var textEntity = _mapper.Map<TextContent>(textDto);
            textEntity.ViolentResult = textDto.ViolentWords.Any()
                ? "Violent Content Flags Triggered"
                : "Safe";
            textEntity.DetectionDate = DateTime.UtcNow;
            textEntity.CreatedAt = DateTime.UtcNow;
            textEntity.URL = textDto.URL;

            await contentRepo.AddAsync(textEntity);
            await AddHistoryRecordAsync(textDto.URL, "Text", textDto.UserEmail);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<TextContentDto>(textEntity);
        }

        public async Task<VideoContentDto> AddVideoContentAsync(CreateVideoContentDto videoDto)
        {
            var contentRepo = _unitOfWork.GetRepository<Content, string>();
            var existing = await contentRepo.GetByIdAsync(videoDto.URL);
            if (existing != null)
                throw new InvalidOperationException("Content for this URL already exists.");

            var videoEntity = _mapper.Map<VideoContent>(videoDto);
            videoEntity.DetectionDate = DateTime.UtcNow;
            videoEntity.CreatedAt = DateTime.UtcNow;
            videoEntity.URL = videoDto.URL;

            await contentRepo.AddAsync(videoEntity);
            await AddHistoryRecordAsync(videoDto.URL, "Video", videoDto.UserEmail);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<VideoContentDto>(videoEntity);
        }

        public Task<byte[]> ConvertVideoFileToBinaryAsync(byte[] videoBytes)
        {
            if (videoBytes is null || videoBytes.Length == 0)
                throw new ArgumentException("Video bytes cannot be empty.", nameof(videoBytes));

            return Task.FromResult(videoBytes);
        }

        private async Task AddHistoryRecordAsync(string contentUrl, string contentType, string userEmail)
        {
            var historyRepo = _unitOfWork.GetRepository<HistoryRecord, string>();
            await historyRepo.AddAsync(new HistoryRecord
            {
                Id = $"HIST-{Guid.NewGuid().ToString()[..8]}",
                ContentUrl = contentUrl,
                ContentType = contentType,
                ActionDate = DateTime.UtcNow,
                AttachedUserEmail = userEmail,
                CreatedAt = DateTime.UtcNow
            });
        }
    }
}

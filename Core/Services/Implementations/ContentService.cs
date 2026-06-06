using AutoMapper;
using Domain.Contracts;
using Domain.Entities.ContentsMudule;
using Domain.Entities.SystemModule;
using Domain.Entities.UserModule;
using Services.Abstraction.Contracts;
using Shared.Dtos.AI_Models;
using Shared.Dtos.Content;
using Shared.Dtos.History;
using Shared.Dtos.Report;
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

            // Filter records based on user's identity email
            var filtered = allContents.Where(c => c.UserEmail.Equals(userEmail, StringComparison.OrdinalIgnoreCase));
            return _mapper.Map<IEnumerable<ContentDto>>(filtered);
        }

        public async Task<TextContentDto> AddTextContentAsync(CreateTextContentDto textDto)
        {
            var repo = _unitOfWork.GetRepository<Content, string>();

            var textEntity = _mapper.Map<TextContent>(textDto);
            textEntity.ViolentResult = textDto.ViolentWords.Any() ? "Violent Content Flags Triggered" : "Safe";
            textEntity.DetectionDate = DateTime.UtcNow;

            await repo.AddAsync(textEntity);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<TextContentDto>(textEntity);
        }

        public async Task<VideoContentDto> AddVideoContentAsync(CreateVideoContentDto videoDto)
        {
            var repo = _unitOfWork.GetRepository<Content, string>();

            var videoEntity = _mapper.Map<VideoContent>(videoDto);
            videoEntity.DetectionDate = DateTime.UtcNow;

            await repo.AddAsync(videoEntity);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<VideoContentDto>(videoEntity);
        }

        public Task<byte[]> ConvertVideoFileToBinaryAsync(byte[] videoBytes)
        {
            throw new NotImplementedException();
        }
    }
}

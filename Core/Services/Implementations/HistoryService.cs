using Domain.Contracts;
using Domain.Entities.ContentsMudule;
using Services.Abstraction.Contracts;
using Shared.Dtos.History;

namespace Services.Implementations
{
    public class HistoryService : IHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public HistoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<HistoryListItemDto>> GetUserHistoryAsync(string userEmail, string typeFilter = "All")
        {
            var repo = _unitOfWork.GetRepository<Content, string>();
            IEnumerable<Content> contents = (await repo.GetAllAsync(asNoTracking: true))
                .Where(c => c.UserEmail.Equals(userEmail, StringComparison.OrdinalIgnoreCase));

            if (!string.Equals(typeFilter, "All", StringComparison.OrdinalIgnoreCase))
            {
                contents = contents.Where(c =>
                    string.Equals(c.ContentType, typeFilter, StringComparison.OrdinalIgnoreCase));
            }

            return contents
                .OrderByDescending(c => c.DetectionDate)
                .Select(MapToListItem);
        }

        public async Task<HistoryDetailsDto?> GetDetailsAsync(string id)
        {
            var repo = _unitOfWork.GetRepository<Content, string>();
            var content = await repo.GetByIdAsync(id);
            if (content is null)
                return null;

            return content switch
            {
                TextContent text => MapTextDetails(text),
                VideoContent video => MapVideoDetails(video),
                _ => null
            };
        }

        public async Task<bool> DeleteRecordAsync(string id)
        {
            var contentRepo = _unitOfWork.GetRepository<Content, string>();
            var content = await contentRepo.GetByIdAsync(id);
            if (content is null)
                return false;

            contentRepo.Delete(content);

            var historyRepo = _unitOfWork.GetRepository<Domain.Entities.SystemModule.HistoryRecord, string>();
            var histories = (await historyRepo.GetAllAsync())
                .Where(h => h.ContentUrl.Equals(id, StringComparison.OrdinalIgnoreCase));
            foreach (var history in histories)
                historyRepo.Delete(history);

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private static HistoryListItemDto MapToListItem(Content content)
        {
            var safetyStatus = GetSafetyStatus(content);
            return new HistoryListItemDto(
                content.Id,
                ExtractDomainName(content.URL),
                content.ContentType,
                FormatRelativeTime(content.DetectionDate),
                safetyStatus);
        }

        private static HistoryDetailsDto MapTextDetails(TextContent text)
        {
            var isViolent = text.ViolentResult.Contains("Violent", StringComparison.OrdinalIgnoreCase);
            var findings = new List<DetailFindingDto>();

            if (!string.IsNullOrWhiteSpace(text.ViolentWords))
            {
                foreach (var word in text.ViolentWords.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                    findings.Add(new DetailFindingDto($"Flagged phrase: {word}", true));
            }

            if (findings.Count == 0)
                findings.Add(new DetailFindingDto(text.textContext, isViolent));

            return new HistoryDetailsDto(
                text.Id,
                text.DetectionDate,
                "Text",
                true,
                text.URL,
                isViolent ? "Violent Content" : "Against Violent Content",
                isViolent ? "Red" : "Green",
                findings);
        }

        private static HistoryDetailsDto MapVideoDetails(VideoContent video)
        {
            var isViolent = video.ViolentPercent > 25.0;
            return new HistoryDetailsDto(
                video.Id,
                video.DetectionDate,
                "Video Stream (MP4)",
                true,
                video.URL,
                isViolent ? "Violent Content" : "Non-Violent Content",
                isViolent ? "Red" : "Green",
                new List<DetailFindingDto>
                {
                    new($"Violence intensity score: {video.ViolentPercent:F1}%", isViolent)
                });
        }

        private static string GetSafetyStatus(Content content) => content switch
        {
            TextContent text when text.ViolentResult.Contains("Violent", StringComparison.OrdinalIgnoreCase) => "Flagged",
            VideoContent video when video.ViolentPercent > 25.0 => "Flagged",
            _ => "Safe"
        };

        private static string ExtractDomainName(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
                return uri.Host + uri.PathAndQuery;

            return url.Length > 40 ? url[..40] + "..." : url;
        }

        private static string FormatRelativeTime(DateTime detectionDate)
        {
            var span = DateTime.UtcNow - detectionDate;
            if (span.TotalMinutes < 60)
                return $"{Math.Max(1, (int)span.TotalMinutes)} minutes ago";
            if (span.TotalHours < 24)
                return $"{(int)span.TotalHours} hours ago";
            if (span.TotalDays < 2)
                return "Yesterday";
            return detectionDate.ToString("dd/MM/yyyy");
        }
    }
}

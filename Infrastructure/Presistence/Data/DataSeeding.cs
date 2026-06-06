using Domain.Contracts;
using Domain.Entities.ContentsMudule;
using Domain.Entities.SystemModule;
using Domain.Entities.UserModule;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Presistence.Data
{
    public class DataSeeding : IDataSeeding
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger<DataSeeding> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public DataSeeding(
            IUnitOfWork unitOfWork,
            IHostEnvironment hostEnvironment,
            ILogger<DataSeeding> logger)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task SeedDataAsync()
        {
            var userRepo = _unitOfWork.GetRepository<User, string>();
            var contentRepo = _unitOfWork.GetRepository<Content, string>();
            var historyRepo = _unitOfWork.GetRepository<HistoryRecord, string>();

            await SeedUsersAsync(userRepo);
            await SeedContentsAsync(userRepo, contentRepo);
            await SeedHistoryAsync(historyRepo);
        }

        private async Task SeedUsersAsync(IGenericRepository<User, string> userRepo)
        {
            if (await userRepo.AnyAsync())
                return;

            var path = ResolveSeedFile("Users.json");
            if (path is null)
                return;

            var items = await DeserializeAsync<List<UserSeedModel>>(path);
            if (items is null || items.Count == 0)
                return;

            var newlyAddedEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var u in items)
            {
                if (string.IsNullOrWhiteSpace(u.Email) || !newlyAddedEmails.Add(u.Email))
                    continue;

                await userRepo.AddAsync(new User
                {
                    Id = u.Email,
                    FullName = u.FullName,
                    Password = u.Password,
                    UserInternalId = Guid.NewGuid().ToString()[..8],
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} users.", newlyAddedEmails.Count);
        }

        private async Task SeedContentsAsync(
            IGenericRepository<User, string> userRepo,
            IGenericRepository<Content, string> contentRepo)
        {
            if (await contentRepo.AnyAsync())
                return;

            var path = ResolveSeedFile("Content.json");
            if (path is null)
                return;

            var items = await DeserializeAsync<List<ContentSeedModel>>(path);
            if (items is null || items.Count == 0)
                return;

            var usersInDb = (await userRepo.GetAllAsync(asNoTracking: true)).ToList();
            var seeded = 0;

            foreach (var c in items)
            {
                var matchingUser = usersInDb.FirstOrDefault(u =>
                    u.Id.Equals(c.UserEmail, StringComparison.OrdinalIgnoreCase));

                if (matchingUser is null)
                {
                    _logger.LogWarning(
                        "Skipping content {ContentId}: user {UserEmail} not found.",
                        c.Id,
                        c.UserEmail);
                    continue;
                }

                if (c.Type.Equals("Video", StringComparison.OrdinalIgnoreCase))
                {
                    await contentRepo.AddAsync(new VideoContent
                    {
                        Id = c.Id,
                        URL = c.URL,
                        ContentType = "Video",
                        UserEmail = matchingUser.Id,
                        ViolentPercent = c.ViolentPercent,
                        DetectionDate = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow
                    });
                    seeded++;
                }
                else if (c.Type.Equals("Text", StringComparison.OrdinalIgnoreCase))
                {
                    await contentRepo.AddAsync(new TextContent
                    {
                        Id = c.Id,
                        URL = c.URL,
                        ContentType = "Text",
                        UserEmail = matchingUser.Id,
                        textContext = c.TextContext,
                        ViolentWords = c.ViolentWords is { Count: > 0 }
                            ? string.Join(", ", c.ViolentWords)
                            : string.Empty,
                        ViolentResult = "Analysed",
                        DetectionDate = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow
                    });
                    seeded++;
                }
            }

            if (seeded > 0)
            {
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Seeded {Count} content records.", seeded);
            }
        }

        private async Task SeedHistoryAsync(IGenericRepository<HistoryRecord, string> historyRepo)
        {
            if (await historyRepo.AnyAsync())
                return;

            var path = ResolveSeedFile("history.json");
            if (path is null)
                return;

            var items = await DeserializeAsync<List<HistoryRecord>>(path);
            if (items is null || items.Count == 0)
                return;

            foreach (var item in items)
            {
                item.CreatedAt = DateTime.UtcNow;
                await historyRepo.AddAsync(item);
            }

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} history records.", items.Count);
        }

        private string? ResolveSeedFile(string fileName)
        {
            var candidates = new[]
            {
                Path.Combine(AppContext.BaseDirectory, "Data", "DataSeed", fileName),
                Path.Combine(AppContext.BaseDirectory, "DataSeed", fileName),
                Path.Combine(
                    _hostEnvironment.ContentRootPath,
                    "..",
                    "Infrastructure",
                    "Presistence",
                    "Data",
                    "DataSeed",
                    fileName)
            };

            foreach (var candidate in candidates)
            {
                var fullPath = Path.GetFullPath(candidate);
                if (File.Exists(fullPath))
                    return fullPath;
            }

            _logger.LogWarning(
                "Seed file {FileName} not found. Checked: {Paths}",
                fileName,
                string.Join(", ", candidates.Select(Path.GetFullPath)));

            return null;
        }

        private async Task<T?> DeserializeAsync<T>(string path)
        {
            try
            {
                var json = await File.ReadAllTextAsync(path);
                return JsonSerializer.Deserialize<T>(json, _jsonOptions);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize seed file {Path}.", path);
                return default;
            }
        }
    }

    public class UserSeedModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class ContentSeedModel
    {
        public string Id { get; set; } = string.Empty;
        public string URL { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public double ViolentPercent { get; set; }
        public string TextContext { get; set; } = string.Empty;
        public List<string>? ViolentWords { get; set; }
    }
}

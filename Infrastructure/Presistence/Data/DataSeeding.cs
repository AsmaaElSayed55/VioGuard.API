using Domain.Contracts;
using Domain.Entities.ContentsMudule;
using Domain.Entities.SystemModule;
using Domain.Entities.SystemModule.ModelsModule;
using Domain.Entities.UserModule;
using System.Text.Json;

namespace Presistence.Data
{
    public class DataSeeding : IDataSeeding
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly JsonSerializerOptions _jsonOptions;

        public DataSeeding(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task SeedDataAsync()
        {
            var systemRepo = _unitOfWork.GetRepository<SystemRoot, string>();
            var modelRepo = _unitOfWork.GetRepository<AIModel, string>();
            var userRepo = _unitOfWork.GetRepository<User, string>();
            var contentRepo = _unitOfWork.GetRepository<Content, string>();
            var historyRepo = _unitOfWork.GetRepository<HistoryRecord, string>();

            // 1. Seed System Roots
            if (!(await systemRepo.GetAllAsync()).Any())
            {
                var path = ResolvePath("systemroot.json");
                if (File.Exists(path))
                {
                    var items = JsonSerializer.Deserialize<List<SystemRoot>>(await File.ReadAllTextAsync(path), _jsonOptions);
                    if (items != null) foreach (var item in items) await systemRepo.AddAsync(item);
                    await _unitOfWork.SaveChangesAsync();
                }
            }

            // 2. Seed AI Models
            if (!(await modelRepo.GetAllAsync()).Any())
            {
                var path = ResolvePath("aimodel.json");
                if (File.Exists(path))
                {
                    var items = JsonSerializer.Deserialize<List<AIModel>>(await File.ReadAllTextAsync(path), _jsonOptions);
                    if (items != null) foreach (var item in items) await modelRepo.AddAsync(item);
                    await _unitOfWork.SaveChangesAsync();
                }
            }

            // 3. Seed Users
            if (!(await userRepo.GetAllAsync()).Any())
            {
                var path = ResolvePath("user.json");
                if (File.Exists(path))
                {
                    var items = JsonSerializer.Deserialize<List<UserSeedModel>>(await File.ReadAllTextAsync(path), _jsonOptions);
                    if (items != null)
                    {
                        foreach (var u in items)
                        {
                            await userRepo.AddAsync(new User
                            {
                           
                                Id = u.Email,
                                FullName = u.FullName,
                                Password = u.Password,
                                UserInternalId = Guid.NewGuid().ToString()[..8]
                            });
                        }
                        await _unitOfWork.SaveChangesAsync();
                    }
                }
            }

            // 4. Seed Contents
            var usersInDb = await userRepo.GetAllAsync();
            if (!(await contentRepo.GetAllAsync()).Any())
            {
                var path = ResolvePath("content.json");
                if (File.Exists(path))
                {
                    var items = JsonSerializer.Deserialize<List<ContentSeedModel>>(await File.ReadAllTextAsync(path), _jsonOptions);
                    if (items != null)
                    {
                        foreach (var c in items)
                        {
                            var matchingUser = usersInDb.FirstOrDefault(u => u.Id.Equals(c.UserEmail, StringComparison.OrdinalIgnoreCase));
                            if (matchingUser == null) continue;

                            if (c.Type.Equals("Video", StringComparison.OrdinalIgnoreCase))
                            {
                                await contentRepo.AddAsync(new VideoContent
                                {
                                    // 💡 FIX: Use c.Id (the "CNT-01" value) instead of forcing the URL string as the ID!
                                    Id = c.Id,
                                    URL = c.URL, // Ensure your VideoContent entity has a separate URL property!
                                    UserEmail = matchingUser.Id,
                                    ViolentPercent = c.ViolentPercent
                                });
                            }
                            else if (c.Type.Equals("Text", StringComparison.OrdinalIgnoreCase))
                            {
                                await contentRepo.AddAsync(new TextContent
                                {
                                    // 💡 FIX: Use c.Id (the "CNT-02" value)
                                    Id = c.Id,
                                    URL = c.URL,
                                    UserEmail = matchingUser.Id,
                                    textContext = c.TextContext,
                                    ViolentWords = c.ViolentWords != null && c.ViolentWords.Any()
                                                     ? string.Join(", ", c.ViolentWords)
                                                     : string.Empty,
                                    ViolentResult = "Analysed"
                                });
                            }
                        }
                        await _unitOfWork.SaveChangesAsync();
                    }
                }
            }

            // 5. Seed History Records
            if (!(await historyRepo.GetAllAsync()).Any())
            {
                var path = ResolvePath("history.json");
                if (File.Exists(path))
                {
                    var items = JsonSerializer.Deserialize<List<HistoryRecord>>(await File.ReadAllTextAsync(path), _jsonOptions);
                    if (items != null) foreach (var item in items) await historyRepo.AddAsync(item);
                    await _unitOfWork.SaveChangesAsync();
                }
            }
        }

        private string ResolvePath(string file)
        {
            var primaryPath = Path.Combine(AppContext.BaseDirectory, "Persistence", "Data", "Seeding", file);
            return File.Exists(primaryPath) ? primaryPath : Path.Combine(AppContext.BaseDirectory, file);
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
        public string Id { get; set; } = string.Empty; // This will be the unique identifier for the content (e.g., "CNT-01")
        public string URL { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public double ViolentPercent { get; set; }
        public string TextContext { get; set; } = string.Empty;
        public List<string>? ViolentWords { get; set; }
    }
}
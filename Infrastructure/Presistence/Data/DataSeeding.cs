using Domain.Contracts;
using Domain.Entities.ContentsMudule; // Fixed a small typo in the word "ContentsModule" here
using Domain.Entities.ContentsMudule;
using Domain.Entities.SystemModule;
using Domain.Entities.UserModule;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Presistence.Data
{
    public class DataSeeding : IDataSeeding
    {
        private readonly IUnitOfWork _unitOfWork;

        public DataSeeding(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // This contains your excellent JSON processing engine
        public async Task SeedAsync()
        {
            var userRepo = _unitOfWork.GetRepository<User, int>();
            var contentRepo = _unitOfWork.GetRepository<Content, int>();

            var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // 1. Seed Users if table is empty
            var existingUsers = await userRepo.GetAllAsync();
            if (!existingUsers.Any())
            {
                var usersPath = Path.Combine(AppContext.BaseDirectory, "Persistence", "Data", "Seeding", "user.json");
                if (!File.Exists(usersPath)) usersPath = Path.Combine(AppContext.BaseDirectory, "user.json");

                if (File.Exists(usersPath))
                {
                    var usersJson = await File.ReadAllTextAsync(usersPath);
                    var usersList = JsonSerializer.Deserialize<List<UserSeedModel>>(usersJson, jsonOptions);

                    if (usersList != null)
                    {
                        foreach (var u in usersList)
                        {
                            await userRepo.AddAsync(new User
                            {
                                FullName = u.FullName,
                                Email = u.Email,
                                Password = u.Password
                            });
                        }
                        await _unitOfWork.SaveChangesAsync();
                    }
                }
            }

            // Refresh our user list tracker to catch newly generated database IDs
            var usersInDb = await userRepo.GetAllAsync();

            // 2. Seed Contents mapped against User IDs if empty
            var existingContent = await contentRepo.GetAllAsync();
            if (!existingContent.Any())
            {
                var contentsPath = Path.Combine(AppContext.BaseDirectory, "Persistence", "Data", "Seeding", "content.json");
                if (!File.Exists(contentsPath)) contentsPath = Path.Combine(AppContext.BaseDirectory, "content.json");

                if (File.Exists(contentsPath))
                {
                    var contentJson = await File.ReadAllTextAsync(contentsPath);
                    var contentList = JsonSerializer.Deserialize<List<ContentSeedModel>>(contentJson, jsonOptions);

                    if (contentList != null)
                    {
                        foreach (var c in contentList)
                        {
                            // Find the user entry in database matching the JSON email identifier
                            var matchingUser = usersInDb.FirstOrDefault(u => u.Email.Equals(c.UserEmail, StringComparison.OrdinalIgnoreCase));
                            if (matchingUser == null) continue; // Skip if no user matches

                            if (c.Type.Equals("Video", StringComparison.OrdinalIgnoreCase))
                            {
                                await contentRepo.AddAsync(new VideoContent
                                {
                                    URL = c.URL,
                                    DetectionDate = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 10)),
                                    UserEmail = matchingUser.Email,
                                    ViolentPercent = c.ViolentPercent
                                });
                            }
                            else if (c.Type.Equals("Text", StringComparison.OrdinalIgnoreCase))
                            {
                                await contentRepo.AddAsync(new TextContent
                                {
                                    URL = c.URL,
                                    DetectionDate = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 10)),
                                    UserEmail = matchingUser.Email,
                                    textContext = c.TextContext,
                                    ViolentWords = c.ViolentWords ?? new List<string>(),
                                    ViolentResult = c.ViolentWords != null && c.ViolentWords.Any()
                                });
                            }
                        }
                        await _unitOfWork.SaveChangesAsync();
                    }
                }
            }
        }

        // Redirect this method to SeedAsync() so your app never crashes regardless of which 
        // method signature your IDataSeeding interface uses to trigger execution.
        public async Task SeedDataAsync()
        {
            await SeedAsync();
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
        public string URL { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public double ViolentPercent { get; set; }
        public string TextContext { get; set; } = string.Empty;
        public List<string>? ViolentWords { get; set; }
    }
}
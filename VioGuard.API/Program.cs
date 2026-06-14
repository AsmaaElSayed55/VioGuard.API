using Domain.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Presentation.Controllers;
using Presentation.Middleware;
using Presistence.Data;
using Presistence.Repositories;
using Services;
using Services.Abstraction.Contracts;
using Services.Implementations;
using System.Text;
using Microsoft.OpenApi;

namespace VioGuard.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Controllers & Swagger Configuration
            builder.Services.AddControllers()
                .AddApplicationPart(typeof(Program).Assembly)
                .AddApplicationPart(typeof(ReportsController).Assembly);

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "VioGuard API", Version = "v1" });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

            });

            // 2. Database Context Registration
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<VioGuardDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddScoped<IApplicationDbContext>(provider =>
                provider.GetRequiredService<VioGuardDbContext>());

            builder.Services.AddScoped<DbContext>(provider =>
                provider.GetRequiredService<VioGuardDbContext>());

            // 3. Repositories & Unit Of Work
            builder.Services.AddScoped<IDataSeeding, DataSeeding>();
            builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // 4. Application Services
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IContentService, ContentService>();
            builder.Services.AddScoped<IContentScrapingService, ContentScrapingService>();
            builder.Services.AddScoped<ITokenService, JwtTokenService>();

            builder.Services.AddScoped<IHistoryService, HistoryService>();
            builder.Services.AddScoped<IReportService, ReportService>();

            builder.Services.AddScoped<IServiceManager, ServiceManager>();

            builder.Services.AddAutoMapper(cfg => { }, typeof(ServicesAssemblyReference).Assembly);

            // 5. HttpClients Configurations
            builder.Services.AddHttpClient("MlService", client =>
            {
                var baseUrl = builder.Configuration["MlService:BaseUrl"] ?? "http://localhost:8000/";
                client.BaseAddress = new Uri(baseUrl);
                client.Timeout = TimeSpan.FromMinutes(5);
            });

            builder.Services.AddHttpClient("ContentScraper", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(builder.Configuration.GetValue("ContentScraping:TimeoutSeconds", 60));
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "*/*");
            });

            builder.Services.AddHttpClient("VideoScraper", client =>
            {
                client.Timeout = TimeSpan.FromMinutes(builder.Configuration.GetValue("ContentScraping:VideoTimeoutMinutes", 5));
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "video/*,application/octet-stream,*/*");
                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "VioGuard/1.0");
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AllowAutoRedirect = true,
                MaxAutomaticRedirections = 10
            });

            // 6. Security, Authentication & Authorization
            var jwtKey = builder.Configuration["Jwt:Key"] ?? "VioGuardSuperSecretKeyForDevelopmentOnly123!";
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "VioGuard",
                    ValidAudience = builder.Configuration["Jwt:Audience"] ?? "VioGuardApp",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    options.DefaultPolicy = new AuthorizationPolicyBuilder()
                        .RequireAssertion(_ => true)
                        .Build();
                }
                else
                {
                    options.DefaultPolicy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                }
            });

            // 7. App Pipelines & Middlewares
            var app = builder.Build();

            app.UseMiddleware<GlobalExceptionMiddleware>();

            await ApplyMigrationsAndSeedAsync(app);

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "VioGuard API v1");
                options.RoutePrefix = "swagger";
                options.DocumentTitle = "VioGuard API";
            });

            if (!app.Environment.IsDevelopment())
              //  app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapGet("/", () => Results.Redirect("/swagger"));
            app.MapControllers();

            app.MapGet("/test-connection", async (VioGuardDbContext db) =>
            {
                try
                {
                    var canConnect = await db.Database.CanConnectAsync();
                    return Results.Ok(new { Success = canConnect, Message = canConnect ? "Database Connected ✔" : "Cannot Connect ❌" });
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            });

            app.Run();
        }

        private static async Task ApplyMigrationsAndSeedAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                var db = scope.ServiceProvider.GetRequiredService<VioGuardDbContext>();
                await db.Database.MigrateAsync();
                logger.LogInformation("Database migrations applied successfully.");

                if (app.Environment.IsDevelopment())
                {
                    var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeding>();
                    await seeder.SeedDataAsync();
                    logger.LogInformation("Development seed data applied successfully.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Database migration or seeding failed.");
            }
        }
    }
}
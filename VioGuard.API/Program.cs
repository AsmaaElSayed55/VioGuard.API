using Domain.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Presentation.Controllers;
using Presistence.Data;
using Presistence.Repositories;
using Services;
using Services.Abstraction.Contracts;
using Services.Implementations;
namespace VioGuard.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers()
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

                //options.AddSecurityRequirement(new OpenApiSecurityRequirement
                //{
                //    {
                //        new OpenApiSecurityScheme
                //        {
                //            Reference = new OpenApiReference
                //            {
                //                Type = ReferenceType.SecurityScheme,
                //                Id = "Bearer"
                //            }
                //        },
                //        Array.Empty<string>()
                //    }
                //});
            });

            // Database Context Configuration
            builder.Services.AddDbContext<VioGuardDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddScoped<IDataSeeding, DataSeeding>();

            builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<VioGuardDbContext>());

            // Data layer registration
            builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IContentService, ContentService>();
            builder.Services.AddScoped<ISystemService, SystemService>();

            builder.Services.AddScoped<IReportService, ReportService>();

            // Architecture Infrastructure mappings
            builder.Services.AddAutoMapper(cfg => { }, typeof(ServicesAssemblyReference).Assembly);
            // Register the Service Manager which handles all services under one hood
            builder.Services.AddScoped<IServiceManager, ServiceManager>();

            builder.Services.AddAuthorization(options =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    options.DefaultPolicy = new AuthorizationPolicyBuilder()
                        .RequireAssertion(_ => true)
                        .Build();
                }
            });


            //builder.Services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            //.AddJwtBearer(options =>
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuer = true,
            //        ValidateAudience = true,
            //        ValidateLifetime = true,
            //        ValidateIssuerSigningKey = true,
            //        ValidIssuer = builder.Configuration["Jwt:Issuer"],
            //        ValidAudience = builder.Configuration["Jwt:Audience"],
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "YourSuperSecretBackupKey123!"))
            //    };
            //});

            var app = builder.Build();

            await ApplyMigrationsAndSeedAsync(app);

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            
            app.MapControllers();

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
                logger.LogInformation("Database migrations applied.");

                if (app.Environment.IsDevelopment())
                {
                    var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeding>();
                    await seeder.SeedDataAsync();
                    logger.LogInformation("Development seed data applied.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Database migration or seeding failed.");
                if (app.Environment.IsDevelopment())
                    throw;
            }
        }
    }
}
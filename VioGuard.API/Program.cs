
using Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using Presistence;
using Presistence.Data;
using Presistence.Repositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace VioGuard.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();


            // 1. Register your DbContext (Make sure this is already there)
            builder.Services.AddDbContext<VioGuardDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // 2. Register the Generic IGenericRepository interface and its implementation
            //   builder.Services.AddScoped(typeof(AssemblyReference).Assembly, typeof(GenericRepository<IGenericRepository, GenericRepository>));

            // 3. Register the Unit of Work (This fixes your exact error)
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // 4. Register the DataSeeding service if you resolve it from services later
            builder.Services.AddScoped<IDataSeeding, DataSeeding>();

            var app = builder.Build();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<VioGuardDbContext>();                    // Ensure database migrations are up to date
                    await context.Database.MigrateAsync();

                    var unitOfWork = services.GetRequiredService<IUnitOfWork>();
                    var seeder = new Infrastructure.Presistence.Data.Seeding.DataSeeding(unitOfWork);
                    await seeder.SeedAsync();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the data infrastructure layer.");
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();


            app.MapControllers();

            app.Run();
        }
    }
}

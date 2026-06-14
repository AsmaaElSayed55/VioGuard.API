using Domain.Entities.ContentsMudule;
using Domain.Entities.SystemModule;
using Domain.Entities.UserModule;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Abstraction.Contracts
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Content> Contents { get; set; }
        DbSet<TextContent> TextContents { get; set; }
        DbSet<VideoContent> VideoContents { get; set; }
        DbSet<HistoryRecord> Histories { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
    }
}
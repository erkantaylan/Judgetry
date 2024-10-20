using Judgetry.Core.Database.Models;
using Judgetry.Web.Database.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Judgetry.Web.Database;

public sealed class JudgetryDbContext : DbContext
{
    public JudgetryDbContext(DbContextOptions<JudgetryDbContext> options) : base(options)
    {
        bool canConnect = Database.CanConnect();
    }

    public DbSet<User> Users { get; set; }
    public DbSet<ReadingEntry> Entries { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        OnCreate();
        OnUpdate();
        OnDelete();

        return base.SaveChangesAsync(cancellationToken);

        void OnCreate()
        {
            IEnumerable<object> createdEntries = ChangeTracker.Entries()
                                                              .Where(x => x.State == EntityState.Added)
                                                              .Select(x => x.Entity);

            foreach (object modifiedEntry in createdEntries)
            {
                if (modifiedEntry is IEntity auditableEntity)
                {
                    auditableEntity.CreatedAt = DateTimeOffset.UtcNow;
                }
            }
        }

        void OnDelete()
        {
            IEnumerable<EntityEntry> deletedEntries = ChangeTracker
                                                     .Entries()
                                                     .Where(x => x.State == EntityState.Deleted);

            foreach (EntityEntry entry in deletedEntries)
            {
                if (entry.Entity is IEntity entity)
                {
                    entity.DeletedAt = DateTimeOffset.UtcNow;
                    entry.State = EntityState.Modified;
                }
            }
        }

        void OnUpdate()
        {
            IEnumerable<object> modifiedEntries = ChangeTracker.Entries()
                                                               .Where(x => x.State == EntityState.Modified)
                                                               .Select(x => x.Entity);

            foreach (object modifiedEntry in modifiedEntries)
            {
                if (modifiedEntry is IEntity auditableEntity)
                {
                    auditableEntity.UpdatedAt = DateTimeOffset.UtcNow;
                }
            }
        }
    }
}

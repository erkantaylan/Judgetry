using Judgetry.Core.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Judgetry.Web.Database;

public class JudgetryDbContext(DbContextOptions<JudgetryDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
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

        return base.SaveChangesAsync(cancellationToken);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using EFCore101.API.Extensions;

public class EFCore101DbContext : DbContext, IEFCore101DbContext
{
    public DbSet<Book> Books { get; set; } = null!;

    public EFCore101DbContext(DbContextOptions<EFCore101DbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new BookConfiguration());

        modelBuilder.ApplyGlobalFilters<ISoftDeleteEntity>(e => !e.IsDeleted);

    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var addedEntries = ChangeTracker.Entries<IAuditableEntity>().Where(e => e.State == EntityState.Added);

        var modifiedEntries = ChangeTracker.Entries<IAuditableEntity>().Where(e => e.State == EntityState.Modified);

        var deletedEntries = ChangeTracker.Entries<ISoftDeleteEntity>().Where(e => e.State == EntityState.Deleted);

        foreach (var entry in addedEntries)
        {
            HandleCreateAuditFields(entry);
        }

        foreach (var entry in modifiedEntries)
        {
            HandleUpdateAuditFields(entry);
        }

        foreach (var entry in deletedEntries)
        {
            HandleDeleteAuditFields(entry);
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    public void HandleCreateAuditFields(EntityEntry<IAuditableEntity> entity)
    {
        if (entity.State == EntityState.Added)
        {
            entity.Entity.CreatedAt = DateTime.UtcNow;
        }
    }

    public void HandleUpdateAuditFields(EntityEntry<IAuditableEntity> entity)
    {
        if (entity.State == EntityState.Modified)
        {
            entity.Entity.UpdatedAt = DateTime.UtcNow;
        }
    }

    public void HandleDeleteAuditFields(EntityEntry<ISoftDeleteEntity> entity)
    {
        if (entity.State == EntityState.Deleted)
        {
            entity.Entity.IsDeleted = true;
        }
    }
}
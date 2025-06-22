using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity<Guid>>())
        {
            HandleCreateAuditFields(entry);
            
            HandleUpdateAuditFields(entry);
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    public void HandleCreateAuditFields(EntityEntry<BaseEntity<Guid>> entity)
    {
        if (entity.State == EntityState.Added)
        {
            entity.Entity.CreatedAt = DateTime.UtcNow;
        }
    }

    public void HandleUpdateAuditFields(EntityEntry<BaseEntity<Guid>> entity)
    {
        if (entity.State == EntityState.Modified)
        {
            entity.Entity.UpdatedAt = DateTime.UtcNow;
        }
    }
}
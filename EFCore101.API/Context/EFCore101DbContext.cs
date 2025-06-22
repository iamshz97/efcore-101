using Microsoft.EntityFrameworkCore;

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
}
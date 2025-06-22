using Microsoft.EntityFrameworkCore;

public class EFCore101DbContext : DbContext
{
    public DbSet<Book> Books { get; set; } = null!;

    public EFCore101DbContext(DbContextOptions<EFCore101DbContext> options)
        : base(options)
    {
    }
}

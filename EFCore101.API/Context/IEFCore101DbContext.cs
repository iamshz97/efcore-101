using Microsoft.EntityFrameworkCore;

public interface IEFCore101DbContext
{
    DbSet<Book> Books { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
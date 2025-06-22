using Microsoft.EntityFrameworkCore;

public interface IEFCore101DbContext
{
    DbSet<Book> Books { get; set; }
    DbSet<Author> Authors { get; set; }
    DbSet<Publisher> Publishers { get; set; }
    DbSet<BookDetails> BookDetails { get; set; }
    DbSet<Category> Categories { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
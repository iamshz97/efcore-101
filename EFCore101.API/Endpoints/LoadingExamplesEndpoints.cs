using Microsoft.EntityFrameworkCore;

namespace EFCore101.API.Endpoints;

/// <summary>
/// Demonstrates different loading strategies in EF Core
/// </summary>
public class LoadingExamplesEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/loading-examples");
        
        // Example 1: Eager Loading with Include
        group.MapGet("/eager-loading", async (IEFCore101DbContext context, CancellationToken cancellationToken) =>
        {
            // Eager loading loads related data in the same query using Include
            var booksWithAuthorsAndPublishers = await context.Books
                .AsNoTracking() // For better performance when you only need to read data
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Details)
                .Include(b => b.Categories)
                .ToListAsync(cancellationToken);
                
            return Results.Ok(new
            {
                Strategy = "Eager Loading",
                Description = "Loads related entities in the same query using Include",
                Advantages = "Single database trip, prevents N+1 query problem",
                Disadvantages = "Can retrieve more data than needed",
                Result = booksWithAuthorsAndPublishers
            });
        });
        
        // Example 2: Lazy Loading
        group.MapGet("/lazy-loading", async (IEFCore101DbContext context, CancellationToken cancellationToken) =>
        {
            // Lazy loading loads related data on-demand when the navigation property is accessed
            // Note: This requires virtual navigation properties and Microsoft.EntityFrameworkCore.Proxies
            var books = await context.Books.ToListAsync(cancellationToken);
            
            // Access navigation properties to trigger lazy loading
            var result = books.Select(book => new
            {
                BookId = book.Id,
                Title = book.Title,
                // These will trigger separate database queries when accessed
                AuthorName = book.Author?.Name,
                PublisherName = book.Publisher?.Name,
                CategoryCount = book.Categories.Count,
                PageCount = book.Details?.NumberOfPages
            }).ToList();
            
            return Results.Ok(new
            {
                Strategy = "Lazy Loading",
                Description = "Loads related entities on-demand when navigation properties are accessed",
                Advantages = "Simple to use, loads only what you access",
                Disadvantages = "Can cause N+1 query problem (performance issue with many entities)",
                Result = result
            });
        });
        
        // Example 3: Explicit Loading
        group.MapGet("/explicit-loading/{bookId}", async (EFCore101DbContext dbContext, Guid bookId, CancellationToken cancellationToken) =>
        {
            // First load the main entity
            var book = await dbContext.Books
                .SingleOrDefaultAsync(b => b.Id == bookId, cancellationToken);
                
            if (book == null)
            {
                return Results.NotFound();
            }
            
            // Then explicitly load related data as needed
            await dbContext.Entry(book)
                .Reference(b => b.Author)
                .LoadAsync(cancellationToken);
                
            await dbContext.Entry(book)
                .Reference(b => b.Details)
                .LoadAsync(cancellationToken);
                
            await dbContext.Entry(book)
                .Collection(b => b.Categories)
                .LoadAsync(cancellationToken);
                
            return Results.Ok(new
            {
                Strategy = "Explicit Loading",
                Description = "Manually control when to load related entities after the main entity is loaded",
                Advantages = "Fine-grained control over what gets loaded and when",
                Disadvantages = "Requires multiple database trips, more code to write",
                Result = book
            });
        });
        
        // Example 4: Projection with Select
        group.MapGet("/projection", async (IEFCore101DbContext context, CancellationToken cancellationToken) =>
        {
            // Projection selects only the specific properties needed
            var bookDtos = await context.Books
                .AsNoTracking()
                .Select(b => new BookSummaryDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    AuthorName = b.Author != null ? b.Author.Name : "Unknown",
                    PublisherName = b.Publisher != null ? b.Publisher.Name : "Unknown",
                    PageCount = b.Details != null ? b.Details.NumberOfPages : 0,
                    Categories = b.Categories.Select(c => c.Name).ToList()
                })
                .ToListAsync(cancellationToken);
                
            return Results.Ok(new
            {
                Strategy = "Projection with Select",
                Description = "Maps entities directly to DTOs, selecting only needed fields",
                Advantages = "Most efficient for read operations, minimal memory usage",
                Disadvantages = "Cannot modify and save the entities (read-only)",
                Result = bookDtos
            });
        });
        
        group.WithOpenApi();
    }
}

// DTO for projection example
public class BookSummaryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string PublisherName { get; set; } = string.Empty;
    public int PageCount { get; set; }
    public List<string> Categories { get; set; } = new();
} 
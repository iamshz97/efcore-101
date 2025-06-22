using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace EFCore101.API.Endpoints;

/// <summary>
/// Demonstrates IQueryable vs IEnumerable and Split Queries in EF Core
/// </summary>
public class QueryableEnumerableEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/queryable-enumerable");
        
        // Example 1: IQueryable (deferred execution)
        group.MapGet("/iqueryable", async (IEFCore101DbContext context, CancellationToken cancellationToken) =>
        {
            // IQueryable represents a query that hasn't been executed yet
            IQueryable<Book> booksQuery = context.Books;
            
            // Each of these operations modifies the query but doesn't execute it
            booksQuery = booksQuery.Where(b => b.Title.Contains("Entity"));
            booksQuery = booksQuery.OrderBy(b => b.Title);
            
            // Query is only executed when we call ToListAsync()
            var books = await booksQuery.ToListAsync(cancellationToken);
            
            // This generates a single SQL query like:
            // SELECT * FROM Books WHERE Title LIKE '%Entity%' ORDER BY Title
            
            return Results.Ok(new
            {
                Type = "IQueryable<T>",
                Description = "Represents a query that hasn't been executed yet",
                Characteristics = new[]
                {
                    "Deferred execution - query only runs when data is needed",
                    "Query can be built up in stages",
                    "Translates to SQL and executes on the database",
                    "Single SQL query is generated at the end"
                },
                ExecutedQuery = "SELECT * FROM Books WHERE Title LIKE '%Entity%' ORDER BY Title",
                Result = books
            });
        });
        
        // Example 2: IEnumerable (immediate execution)
        group.MapGet("/ienumerable", async (IEFCore101DbContext context, CancellationToken cancellationToken) =>
        {
            // This executes the query immediately, returning all books from database
            IEnumerable<Book> booksEnumerable = await context.Books.ToListAsync(cancellationToken);
            
            // These operations happen in memory, not in the database
            booksEnumerable = booksEnumerable.Where(b => b.Title.Contains("Entity"));
            booksEnumerable = booksEnumerable.OrderBy(b => b.Title);
            
            // Convert to list for the result
            var books = booksEnumerable.ToList();
            
            return Results.Ok(new
            {
                Type = "IEnumerable<T>",
                Description = "Represents a collection that has already been loaded into memory",
                Characteristics = new[]
                {
                    "Immediate execution - data is loaded when ToList() is called",
                    "Further operations happen in memory, not in the database",
                    "Less efficient for large datasets",
                    "Multiple operations generate multiple memory iterations"
                },
                ExecutedQuery = "SELECT * FROM Books -- then filtering happens in memory",
                Result = books
            });
        });
        
        // Example 3: Comparing IQueryable and IEnumerable
        group.MapGet("/comparison", async (IEFCore101DbContext context, CancellationToken cancellationToken) =>
        {
            // IQueryable approach - single database query
            var startTimeQueryable = DateTime.UtcNow;
            var booksQueryable = await context.Books
                .Where(b => b.Title.Contains("Entity"))
                .OrderBy(b => b.Title)
                .Take(10)
                .ToListAsync(cancellationToken);
            var endTimeQueryable = DateTime.UtcNow;
            
            // IEnumerable approach - loads all data first
            var startTimeEnumerable = DateTime.UtcNow;
            var allBooks = await context.Books.ToListAsync(cancellationToken);
            var booksEnumerable = allBooks
                .Where(b => b.Title.Contains("Entity"))
                .OrderBy(b => b.Title)
                .Take(10)
                .ToList();
            var endTimeEnumerable = DateTime.UtcNow;
            
            return Results.Ok(new
            {
                Comparison = new
                {
                    IQueryable = new
                    {
                        ExecutionTime = (endTimeQueryable - startTimeQueryable).TotalMilliseconds,
                        ExecutedQuery = "SELECT TOP(10) * FROM Books WHERE Title LIKE '%Entity%' ORDER BY Title",
                        ResultCount = booksQueryable.Count
                    },
                    IEnumerable = new
                    {
                        ExecutionTime = (endTimeEnumerable - startTimeEnumerable).TotalMilliseconds,
                        ExecutedQuery = "SELECT * FROM Books -- then filtering in memory",
                        ResultCount = booksEnumerable.Count
                    }
                },
                Recommendation = "Use IQueryable when working with databases to leverage SQL optimization"
            });
        });
        
        // Example 4: Split Queries for complex includes
        group.MapGet("/split-queries", async (IEFCore101DbContext context, CancellationToken cancellationToken) =>
        {
            // Single query approach (may cause cartesian explosion with multiple includes)
            var startTimeSingle = DateTime.UtcNow;
            var booksWithSingleQuery = await context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Details)
                .Include(b => b.Categories)
                .ToListAsync(cancellationToken);
            var endTimeSingle = DateTime.UtcNow;
            
            // Split query approach (multiple queries, but potentially more efficient)
            var startTimeSplit = DateTime.UtcNow;
            var booksWithSplitQueries = await context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Details)
                .Include(b => b.Categories)
                .AsSplitQuery() // This is the key difference
                .ToListAsync(cancellationToken);
            var endTimeSplit = DateTime.UtcNow;
            
            return Results.Ok(new
            {
                SplitQueries = new
                {
                    Description = "Split Queries in EF Core",
                    SingleQueryApproach = new
                    {
                        ExecutionTime = (endTimeSingle - startTimeSingle).TotalMilliseconds,
                        Characteristics = new[]
                        {
                            "Loads all data in a single SQL query",
                            "Can cause cartesian explosion with multiple includes",
                            "May return duplicate data that needs to be filtered",
                            "Better for smaller result sets"
                        },
                        BookCount = booksWithSingleQuery.Count
                    },
                    SplitQueryApproach = new
                    {
                        ExecutionTime = (endTimeSplit - startTimeSplit).TotalMilliseconds,
                        Characteristics = new[]
                        {
                            "Executes separate SQL queries for each include",
                            "Avoids cartesian explosion problem",
                            "More efficient for large datasets with multiple includes",
                            "Slightly more network overhead due to multiple queries"
                        },
                        BookCount = booksWithSplitQueries.Count,
                        GeneratedQueries = new[]
                        {
                            "Query 1: SELECT * FROM Books",
                            "Query 2: SELECT * FROM Authors WHERE Id IN (...)",
                            "Query 3: SELECT * FROM Publishers WHERE Id IN (...)",
                            "Query 4: SELECT * FROM BookDetails WHERE BookId IN (...)",
                            "Query 5: SELECT * FROM BookCategories bc JOIN Categories c ON bc.CategoryId = c.Id WHERE bc.BookId IN (...)"
                        }
                    },
                    Recommendation = "Use AsSplitQuery() for queries with multiple includes, especially for large datasets"
                }
            });
        });
        
        // Example 5: AsSingleQuery vs AsSplitQuery
        group.MapGet("/query-types/{bookId}", async (IEFCore101DbContext context, Guid bookId, CancellationToken cancellationToken) =>
        {
            // Default behavior (depends on EF Core version and configuration)
            var defaultBook = await context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Details)
                .Include(b => b.Categories)
                .FirstOrDefaultAsync(b => b.Id == bookId, cancellationToken);
                
            // Explicitly as single query
            var singleQueryBook = await context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Details)
                .Include(b => b.Categories)
                .AsSingleQuery() // Explicit single query
                .FirstOrDefaultAsync(b => b.Id == bookId, cancellationToken);
                
            // Explicitly as split query
            var splitQueryBook = await context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Details)
                .Include(b => b.Categories)
                .AsSplitQuery() // Explicit split query
                .FirstOrDefaultAsync(b => b.Id == bookId, cancellationToken);
                
            if (defaultBook == null)
            {
                return Results.NotFound();
            }
                
            return Results.Ok(new
            {
                QueryTypes = new
                {
                    DefaultBehavior = "Depends on EF Core version and configuration",
                    AsSingleQuery = "Forces a single SQL query with JOINs",
                    AsSplitQuery = "Forces separate SQL queries for each included relationship",
                    WhenToUse = new
                    {
                        SingleQuery = "Small to medium result sets with few relationships",
                        SplitQuery = "Large result sets or queries with many relationships"
                    },
                    GlobalConfiguration = "You can configure the default in DbContext options: options.UseSqlServer(connectionString, o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))"
                }
            });
        });
        
        group.WithOpenApi();
    }
} 
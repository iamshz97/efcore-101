using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace EFCore101.API.Endpoints;

/// <summary>
/// Demonstrates client vs server evaluation in EF Core
/// </summary>
public class ClientServerEvaluationEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/evaluation-examples");
        
        // Example 1: Server evaluation (good)
        group.MapGet("/server-evaluation", async (IEFCore101DbContext context, CancellationToken cancellationToken) =>
        {
            // This will be translated to SQL and executed on the server
            var books = await context.Books
                .Where(b => b.Title.Contains("Entity Framework"))
                .OrderBy(b => b.Title)
                .ToListAsync(cancellationToken);
                
            return Results.Ok(new
            {
                EvaluationType = "Server Evaluation",
                Description = "Query is translated to SQL and executed entirely on the database server",
                Advantages = "Efficient, uses database indexes, minimizes data transfer",
                ExecutedQuery = "SELECT * FROM Books WHERE Title LIKE '%Entity Framework%' ORDER BY Title",
                Result = books
            });
        });
        
        // Example 2: Client evaluation (potentially problematic)
        group.MapGet("/client-evaluation", async (IEFCore101DbContext context, CancellationToken cancellationToken) =>
        {
            // This custom method can't be translated to SQL, forcing client evaluation
            static bool ContainsIgnoreCase(string source, string value)
            {
                return source?.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
            }
            
            // EF Core will fetch ALL books and then filter in memory
            var books = await context.Books
                .AsNoTracking()
                .ToListAsync(cancellationToken);
                
            var filteredBooks = books
                .Where(b => ContainsIgnoreCase(b.Title, "entity framework"))
                .OrderBy(b => b.Title)
                .ToList();
                
            return Results.Ok(new
            {
                EvaluationType = "Client Evaluation",
                Description = "Part or all of the query is executed in application memory",
                Disadvantages = "Inefficient for large datasets, loads unnecessary data, bypasses database indexes",
                ExecutedQuery = "SELECT * FROM Books -- then filtering happens in application memory",
                Result = filteredBooks
            });
        });
        
        // Example 3: Mixed evaluation (some server, some client)
        group.MapGet("/mixed-evaluation", async (IEFCore101DbContext context, CancellationToken cancellationToken) =>
        {
            // First part runs on server
            var books = await context.Books
                .Where(b => b.Title.Contains("Entity"))
                .ToListAsync(cancellationToken);
                
            // Second part runs on client
            var filteredBooks = books
                .Where(b => b.Description.Split(' ').Length > 50) // Complex operation runs on client
                .OrderBy(b => b.Title)
                .ToList();
                
            return Results.Ok(new
            {
                EvaluationType = "Mixed Evaluation",
                Description = "Part of the query runs on the server, part runs on the client",
                Explanation = "The WHERE Title CONTAINS 'Entity' runs on server, but the complex string operation runs on client",
                ExecutedQuery = "SELECT * FROM Books WHERE Title LIKE '%Entity%' -- then additional filtering in memory",
                Result = filteredBooks
            });
        });
        
        // Example 4: Forcing client evaluation (explicit)
        group.MapGet("/explicit-client-evaluation", async (IEFCore101DbContext context, CancellationToken cancellationToken) =>
        {
            // AsEnumerable() forces client evaluation for the rest of the query
            var books = await context.Books
                .Where(b => b.Title.Contains("Entity")) // This part runs on server
                .ToListAsync(cancellationToken); // Execute the query and get results from server
                
            // Now perform client-side operations
            var filteredBooks = books
                .AsEnumerable() // Not necessary but explicit
                .Where(b => b.Description.ToLower().Contains("framework"))
                .OrderBy(b => b.Title)
                .ToList(); // Regular ToList() for IEnumerable
                
            return Results.Ok(new
            {
                EvaluationType = "Explicit Client Evaluation",
                Description = "Deliberately forcing part of the query to run on the client",
                Explanation = "AsEnumerable() switches from IQueryable to IEnumerable, forcing client evaluation",
                ExecutedQuery = "SELECT * FROM Books WHERE Title LIKE '%Entity%' -- then additional operations in memory",
                WhenToUse = "When you need to use methods that can't be translated to SQL",
                Result = filteredBooks
            });
        });
        
        // Example 5: EF.Functions for better server evaluation
        group.MapGet("/ef-functions", async (IEFCore101DbContext context, CancellationToken cancellationToken) =>
        {
            // Using EF.Functions for better SQL translation
            var books = await context.Books
                .Where(b => EF.Functions.Like(b.Title, "%Entity%"))
                .Where(b => EF.Functions.Like(b.Description, "%Framework%"))
                .OrderBy(b => b.Title)
                .ToListAsync(cancellationToken);
                
            return Results.Ok(new
            {
                EvaluationType = "EF.Functions Server Evaluation",
                Description = "Using EF.Functions to ensure server evaluation with optimized SQL",
                Advantages = "Efficient, runs entirely on server, uses database-specific optimizations",
                ExecutedQuery = "SELECT * FROM Books WHERE Title LIKE '%Entity%' AND Description LIKE '%Framework%' ORDER BY Title",
                Result = books
            });
        });
        
        group.WithOpenApi();
    }
} 
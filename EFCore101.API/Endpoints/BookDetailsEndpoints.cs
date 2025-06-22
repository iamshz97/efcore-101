using Microsoft.EntityFrameworkCore;

namespace EFCore101.API.Endpoints;

public class BookDetailsEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/book-details");

        group.MapPost("/", async (IEFCore101DbContext context, BookDetailsRequest request, CancellationToken cancellationToken) =>
        {
            var book = await context.Books.FindAsync(request.BookId, cancellationToken);
            if (book == null)
            {
                return Results.NotFound($"Book with ID {request.BookId} not found");
            }

            var existingDetails = await context.BookDetails
                .FirstOrDefaultAsync(bd => bd.BookId == request.BookId, cancellationToken);
            
            if (existingDetails != null)
            {
                return Results.Conflict($"Book details already exist for book with ID {request.BookId}");
            }

            var bookDetails = new BookDetails
            {
                BookId = request.BookId,
                NumberOfPages = request.NumberOfPages,
                Language = request.Language,
                ISBN = request.ISBN,
                PublishedDate = request.PublishedDate
            };

            await context.BookDetails.AddAsync(bookDetails, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return Results.Created($"/book-details/{bookDetails.Id}", bookDetails);
        });

        group.MapGet("/", async (IEFCore101DbContext context, CancellationToken cancellationToken) =>
        {
            var bookDetails = await context.BookDetails.ToListAsync(cancellationToken);
            return Results.Ok(bookDetails);
        });

        group.MapGet("/{id}", async (IEFCore101DbContext context, Guid id, CancellationToken cancellationToken) =>
        {
            var bookDetails = await context.BookDetails.FindAsync(id, cancellationToken);
            
            if (bookDetails == null)
            {
                return Results.NotFound();
            }
            
            return Results.Ok(bookDetails);
        });

        group.MapGet("/book/{bookId}", async (IEFCore101DbContext context, Guid bookId, CancellationToken cancellationToken) =>
        {
            var bookDetails = await context.BookDetails
                .Include(bd => bd.Book)
                .FirstOrDefaultAsync(bd => bd.BookId == bookId, cancellationToken);
            
            if (bookDetails == null)
            {
                return Results.NotFound();
            }
            
            return Results.Ok(bookDetails);
        });

        group.MapPut("/{id}", async (IEFCore101DbContext context, Guid id, BookDetailsRequest request, CancellationToken cancellationToken) =>
        {
            var bookDetails = await context.BookDetails.FindAsync(id, cancellationToken);
            
            if (bookDetails == null)
            {
                return Results.NotFound();
            }

            if (bookDetails.BookId != request.BookId)
            {
                var newBook = await context.Books.FindAsync(request.BookId, cancellationToken);
                if (newBook == null)
                {
                    return Results.NotFound($"Book with ID {request.BookId} not found");
                }

                var existingDetails = await context.BookDetails
                    .FirstOrDefaultAsync(bd => bd.BookId == request.BookId, cancellationToken);
                
                if (existingDetails != null && existingDetails.Id != id)
                {
                    return Results.Conflict($"Book details already exist for book with ID {request.BookId}");
                }
            }

            bookDetails.BookId = request.BookId;
            bookDetails.NumberOfPages = request.NumberOfPages;
            bookDetails.Language = request.Language;
            bookDetails.ISBN = request.ISBN;
            bookDetails.PublishedDate = request.PublishedDate;
            
            await context.SaveChangesAsync(cancellationToken);
            
            return Results.Ok(bookDetails);
        });

        group.MapDelete("/{id}", async (IEFCore101DbContext context, Guid id, CancellationToken cancellationToken) =>
        {
            var bookDetails = await context.BookDetails.FindAsync(id, cancellationToken);

            if (bookDetails == null)
            {
                return Results.NotFound();
            }

            context.BookDetails.Remove(bookDetails);
            await context.SaveChangesAsync(cancellationToken);
            
            return Results.NoContent();
        });

        group.WithOpenApi();
    }
}

public record BookDetailsRequest(
    Guid BookId, 
    int NumberOfPages, 
    string? Language, 
    string? ISBN, 
    DateTime? PublishedDate
); 
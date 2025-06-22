using Microsoft.EntityFrameworkCore;

namespace EFCore101.API.Endpoints;

public class BookEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/books");

        group.MapPost("/", async (IEFCore101DbContext context, BookRequest request, CancellationToken cancellationToken) =>
        {
            var book = new Book
            {
                Title = request.Title,
                Description = request.Description,
                ImageUrl = request.ImageUrl
            };

            await context.Books.AddAsync(book, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return Results.Created($"/books/{book.Id}", book);
        });

        group.MapGet("/", async (IEFCore101DbContext context, CancellationToken cancellationToken) =>
        {
            var books = await context.Books.ToListAsync(cancellationToken);
            return Results.Ok(books);
        });

        group.MapGet("/{id}", async (IEFCore101DbContext context, Guid id, CancellationToken cancellationToken) =>
        {
            var book = await context.Books.FindAsync(id, cancellationToken);
            return Results.Ok(book);
        });

        group.MapDelete("/{id}", async (IEFCore101DbContext context, Guid id, CancellationToken cancellationToken) =>
        {
            var book = await context.Books.FindAsync(id, cancellationToken);

            if (book == null)
            {
                return Results.NotFound();
            }

            context.Books.Remove(book);
            await context.SaveChangesAsync(cancellationToken);
            
            return Results.NoContent();
        });

        group.WithOpenApi();
    }
}

public record BookRequest(string Title, string Description, string ImageUrl); 
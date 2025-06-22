using Microsoft.EntityFrameworkCore;

namespace EFCore101.API.Endpoints;

public class AuthorEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/authors");

        group.MapPost("/", async (IEFCore101DbContext context, AuthorRequest request, CancellationToken cancellationToken) =>
        {
            var author = new Author
            {
                Name = request.Name,
                Biography = request.Biography,
                Email = request.Email
            };

            await context.Authors.AddAsync(author, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return Results.Created($"/authors/{author.Id}", author);
        });

        group.MapGet("/", async (IEFCore101DbContext context, CancellationToken cancellationToken) =>
        {
            var authors = await context.Authors.ToListAsync(cancellationToken);
            return Results.Ok(authors);
        });

        group.MapGet("/{id}", async (IEFCore101DbContext context, Guid id, CancellationToken cancellationToken) =>
        {
            var author = await context.Authors.FindAsync(id, cancellationToken);
            
            if (author == null)
            {
                return Results.NotFound();
            }
            
            return Results.Ok(author);
        });

        group.MapGet("/{id}/books", async (IEFCore101DbContext context, Guid id, CancellationToken cancellationToken) =>
        {
            var author = await context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
            
            if (author == null)
            {
                return Results.NotFound();
            }
            
            return Results.Ok(author);
        });

        group.MapPut("/{id}", async (IEFCore101DbContext context, Guid id, AuthorRequest request, CancellationToken cancellationToken) =>
        {
            var author = await context.Authors.FindAsync(id, cancellationToken);
            
            if (author == null)
            {
                return Results.NotFound();
            }

            author.Name = request.Name;
            author.Biography = request.Biography;
            author.Email = request.Email;
            
            await context.SaveChangesAsync(cancellationToken);
            
            return Results.Ok(author);
        });

        group.MapDelete("/{id}", async (IEFCore101DbContext context, Guid id, CancellationToken cancellationToken) =>
        {
            var author = await context.Authors.FindAsync(id, cancellationToken);

            if (author == null)
            {
                return Results.NotFound();
            }

            context.Authors.Remove(author);
            await context.SaveChangesAsync(cancellationToken);
            
            return Results.NoContent();
        });

        group.WithOpenApi();
    }
}

public record AuthorRequest(string Name, string? Biography, string? Email); 
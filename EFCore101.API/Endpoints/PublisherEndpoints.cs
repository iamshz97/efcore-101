using Microsoft.EntityFrameworkCore;

namespace EFCore101.API.Endpoints;

public class PublisherEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/publishers");

        group.MapPost("/", async (IEFCore101DbContext context, PublisherRequest request, CancellationToken cancellationToken) =>
        {
            var publisher = new Publisher
            {
                Name = request.Name,
                Website = request.Website,
                Location = request.Location
            };

            await context.Publishers.AddAsync(publisher, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return Results.Created($"/publishers/{publisher.Id}", publisher);
        });

        group.MapGet("/", async (IEFCore101DbContext context, CancellationToken cancellationToken) =>
        {
            var publishers = await context.Publishers.ToListAsync(cancellationToken);
            return Results.Ok(publishers);
        });

        group.MapGet("/{id}", async (IEFCore101DbContext context, Guid id, CancellationToken cancellationToken) =>
        {
            var publisher = await context.Publishers.FindAsync(id, cancellationToken);
            
            if (publisher == null)
            {
                return Results.NotFound();
            }
            
            return Results.Ok(publisher);
        });

        group.MapGet("/{id}/books", async (IEFCore101DbContext context, Guid id, CancellationToken cancellationToken) =>
        {
            var publisher = await context.Publishers
                .Include(p => p.Books)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            
            if (publisher == null)
            {
                return Results.NotFound();
            }
            
            return Results.Ok(publisher);
        });

        group.MapPut("/{id}", async (IEFCore101DbContext context, Guid id, PublisherRequest request, CancellationToken cancellationToken) =>
        {
            var publisher = await context.Publishers.FindAsync(id, cancellationToken);
            
            if (publisher == null)
            {
                return Results.NotFound();
            }

            publisher.Name = request.Name;
            publisher.Website = request.Website;
            publisher.Location = request.Location;
            
            await context.SaveChangesAsync(cancellationToken);
            
            return Results.Ok(publisher);
        });

        group.MapDelete("/{id}", async (IEFCore101DbContext context, Guid id, CancellationToken cancellationToken) =>
        {
            var publisher = await context.Publishers.FindAsync(id, cancellationToken);

            if (publisher == null)
            {
                return Results.NotFound();
            }

            context.Publishers.Remove(publisher);
            await context.SaveChangesAsync(cancellationToken);
            
            return Results.NoContent();
        });

        group.WithOpenApi();
    }
}

public record PublisherRequest(string Name, string? Website, string? Location); 
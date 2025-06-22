using Microsoft.EntityFrameworkCore;

namespace EFCore101.API.Endpoints;

public class CategoryEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/categories");

        group.MapPost("/", async (IEFCore101DbContext context, CategoryRequest request, CancellationToken cancellationToken) =>
        {
            var category = new Category
            {
                Name = request.Name,
                Description = request.Description
            };

            await context.Categories.AddAsync(category, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return Results.Created($"/categories/{category.Id}", category);
        });

        group.MapGet("/", async (IEFCore101DbContext context, CancellationToken cancellationToken) =>
        {
            var categories = await context.Categories.ToListAsync(cancellationToken);
            return Results.Ok(categories);
        });

        group.MapGet("/{id}", async (IEFCore101DbContext context, int id, CancellationToken cancellationToken) =>
        {
            var category = await context.Categories.FindAsync(id, cancellationToken);
            
            if (category == null)
            {
                return Results.NotFound();
            }
            
            return Results.Ok(category);
        });

        group.MapPut("/{id}", async (IEFCore101DbContext context, int id, CategoryRequest request, CancellationToken cancellationToken) =>
        {
            var category = await context.Categories.FindAsync(id, cancellationToken);
            
            if (category == null)
            {
                return Results.NotFound();
            }

            category.Name = request.Name;
            category.Description = request.Description;
            
            await context.SaveChangesAsync(cancellationToken);
            
            return Results.Ok(category);
        });

        group.MapDelete("/{id}", async (IEFCore101DbContext context, int id, CancellationToken cancellationToken) =>
        {
            var category = await context.Categories.FindAsync(id, cancellationToken);

            if (category == null)
            {
                return Results.NotFound();
            }

            context.Categories.Remove(category);
            await context.SaveChangesAsync(cancellationToken);
            
            return Results.NoContent();
        });

        group.WithOpenApi();
    }
}

public record CategoryRequest(string Name, string? Description); 
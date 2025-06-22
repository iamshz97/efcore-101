using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddOpenApi();

builder.AddNpgsqlDbContext<EFCore101DbContext>(connectionName: "postgresdb");

builder.Services.AddScoped<IEFCore101DbContext, EFCore101DbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options => options
    .WithTitle("EF Core 101 API")
    .WithTheme(ScalarTheme.Saturn)
    .WithDarkMode());

    // Run Auto migration
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<EFCore101DbContext>();
        await dbContext.Database.MigrateAsync();
    }
}

app.UseHttpsRedirection();

app.MapPost("/books", async (IEFCore101DbContext context, BookRequest request, CancellationToken cancellationToken) =>
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

app.MapGet("/books", async (IEFCore101DbContext context, CancellationToken cancellationToken) =>
{
    var books = await context.Books.ToListAsync(cancellationToken);
    return Results.Ok(books);
});

app.MapGet("/books/{id}", async (IEFCore101DbContext context, Guid id, CancellationToken cancellationToken) =>
{
    var book = await context.Books.FindAsync(id, cancellationToken);
    return Results.Ok(book);
});

app.MapDelete("/books/{id}", async (IEFCore101DbContext context, Guid id, CancellationToken cancellationToken) =>
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

app.Run();

public record BookRequest(string Title, string Description, string ImageUrl);

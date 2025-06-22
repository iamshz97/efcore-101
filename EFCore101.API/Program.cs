using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using EFCore101.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenApi();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddOpenApi();

builder.AddNpgsqlDbContext<EFCore101DbContext>(connectionName: "postgresdb");

builder.Services.AddScoped<IEFCore101DbContext, EFCore101DbContext>();

builder.Services.AddEndpoints(typeof(Program).Assembly);

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

app.MapEndpoints();

app.UseHttpsRedirection();

app.Run();

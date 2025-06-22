using Microsoft.EntityFrameworkCore;

namespace EFCore101.API.Seeds;

/// <summary>
/// Provides seed data for book categories
/// </summary>
public static class CategorySeed
{
    /// <summary>
    /// Seeds the database with predefined categories
    /// </summary>
    /// <param name="modelBuilder">The model builder instance</param>
    public static void SeedCategories(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(
            new Category
            {
                Id = 1,
                Name = "Fiction",
                Description = "Literary works based on imagination rather than fact",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Category
            {
                Id = 2,
                Name = "Non-Fiction",
                Description = "Literary works based on factual information",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Category
            {
                Id = 3,
                Name = "Science Fiction",
                Description = "Fiction based on imagined future scientific or technological advances",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Category
            {
                Id = 4,
                Name = "Fantasy",
                Description = "Fiction involving magical elements and imaginary worlds",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Category
            {
                Id = 5,
                Name = "Mystery",
                Description = "Fiction dealing with the solution of a crime or puzzle",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Category
            {
                Id = 6,
                Name = "Biography",
                Description = "An account of someone's life written by someone else",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Category
            {
                Id = 7,
                Name = "History",
                Description = "Books about past events and human societies",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Category
            {
                Id = 8,
                Name = "Self-Help",
                Description = "Books aimed at helping readers improve their lives",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Category
            {
                Id = 9,
                Name = "Programming",
                Description = "Books about computer programming and software development",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Category
            {
                Id = 10,
                Name = "Business",
                Description = "Books about business management, entrepreneurship, and economics",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        );
    }
} 
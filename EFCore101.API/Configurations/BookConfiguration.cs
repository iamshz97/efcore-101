using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCore101.API.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable(nameof(Book));

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasComment("The unique identifier of the book");

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(255)
            .HasComment("The title of the book");

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(1000)
            .HasComment("The description of the book");

        builder.Property(e => e.ImageUrl)
            .IsRequired()
            .HasComment("URL to the book's cover image");

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasComment("The date and time the book was created");

        builder.Property(e => e.UpdatedAt)
            .IsRequired()
            .HasComment("The date and time the book was updated");

        // Define relationships - these are also defined in the other entity configurations
        // but defining them here as well ensures the relationship is properly configured from both sides

        // One-to-many relationship with Author
        builder.HasOne(b => b.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(b => b.AuthorId)
            .IsRequired(false) // Author is optional
            .OnDelete(DeleteBehavior.SetNull);

        // One-to-many relationship with Publisher
        builder.HasOne(b => b.Publisher)
            .WithMany(p => p.Books)
            .HasForeignKey(b => b.PublisherId)
            .IsRequired(false) // Publisher is optional
            .OnDelete(DeleteBehavior.SetNull);

        // One-to-one relationship with BookDetails
        // This is defined in BookDetailsConfiguration, but we can also define it here
        builder.HasOne(b => b.Details)
            .WithOne(d => d.Book)
            .HasForeignKey<BookDetails>(d => d.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        // Many-to-many relationship with Category
        // This is defined in CategoryConfiguration, but we can also define it here
        builder.HasMany(b => b.Categories)
            .WithMany(c => c.Books)
            .UsingEntity(
                "BookCategory",
                l => l.HasOne(typeof(Category)).WithMany().HasForeignKey("CategoryId"),
                r => r.HasOne(typeof(Book)).WithMany().HasForeignKey("BookId"),
                j => j.HasKey("BookId", "CategoryId"));
    }
}
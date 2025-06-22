using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCore101.API.Configurations;

public class BookDetailsConfiguration : IEntityTypeConfiguration<BookDetails>
{
    public void Configure(EntityTypeBuilder<BookDetails> builder)
    {
        builder.ToTable(nameof(BookDetails));

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasComment("The unique identifier of the book details");

        builder.Property(e => e.NumberOfPages)
            .HasComment("Number of pages in the book");

        builder.Property(e => e.Language)
            .HasMaxLength(50)
            .HasComment("The language the book is written in");

        builder.Property(e => e.ISBN)
            .HasMaxLength(20)
            .HasComment("The ISBN (International Standard Book Number)");

        builder.Property(e => e.PublishedDate)
            .HasComment("The publication date of the book");

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasComment("The date and time the book details were created");

        builder.Property(e => e.UpdatedAt)
            .IsRequired()
            .HasComment("The date and time the book details were updated");

        // Define one-to-one relationship with Book
        builder.HasOne(d => d.Book)
            .WithOne(b => b.Details)
            .HasForeignKey<BookDetails>(d => d.BookId)
            .OnDelete(DeleteBehavior.Cascade) // When book is deleted, delete its details too
            .HasConstraintName("FK_BookDetails_Book");

        // Make BookId the primary key to enforce the one-to-one relationship
        builder.HasKey(d => d.BookId);
    }
} 
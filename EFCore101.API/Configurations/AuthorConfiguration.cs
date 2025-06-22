using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCore101.API.Configurations;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.ToTable(nameof(Author));

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasComment("The unique identifier of the author");

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("The full name of the author");

        builder.Property(e => e.Biography)
            .HasMaxLength(2000)
            .HasComment("A short biography of the author");

        builder.Property(e => e.Email)
            .HasMaxLength(150)
            .HasComment("The author's email address");

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasComment("The date and time the author was created");

        builder.Property(e => e.UpdatedAt)
            .IsRequired()
            .HasComment("The date and time the author was updated");

        // Define one-to-many relationship with Book
        builder.HasMany(a => a.Books)
            .WithOne(b => b.Author)
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.SetNull) // When author is deleted, set book's AuthorId to null
            .HasConstraintName("FK_Book_Author");
    }
} 
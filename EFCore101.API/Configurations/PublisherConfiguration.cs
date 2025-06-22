using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCore101.API.Configurations;

public class PublisherConfiguration : IEntityTypeConfiguration<Publisher>
{
    public void Configure(EntityTypeBuilder<Publisher> builder)
    {
        builder.ToTable(nameof(Publisher));

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasComment("The unique identifier of the publisher");

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasComment("The name of the publishing company");

        builder.Property(e => e.Website)
            .HasMaxLength(255)
            .HasComment("The publisher's website URL");

        builder.Property(e => e.Location)
            .HasMaxLength(150)
            .HasComment("The publisher's headquarters location");

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasComment("The date and time the publisher was created");

        builder.Property(e => e.UpdatedAt)
            .IsRequired()
            .HasComment("The date and time the publisher was updated");

        // Define one-to-many relationship with Book
        builder.HasMany(p => p.Books)
            .WithOne(b => b.Publisher)
            .HasForeignKey(b => b.PublisherId)
            .OnDelete(DeleteBehavior.SetNull) // When publisher is deleted, set book's PublisherId to null
            .HasConstraintName("FK_Book_Publisher");
    }
} 
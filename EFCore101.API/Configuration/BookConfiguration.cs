using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasComment("The date and time the book was created");

        builder.Property(e => e.UpdatedAt)
            .IsRequired()
            .HasComment("The date and time the book was updated");
    }
}
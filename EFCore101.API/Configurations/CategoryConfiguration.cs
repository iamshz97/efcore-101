using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCore101.API.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable(nameof(Category));

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasComment("The unique identifier of the category");

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("The name of the category");

        builder.Property(e => e.Description)
            .HasMaxLength(500)
            .HasComment("A description of the category");

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasComment("The date and time the category was created");

        builder.Property(e => e.UpdatedAt)
            .IsRequired()
            .HasComment("The date and time the category was updated");

        // Define many-to-many relationship with Book
        builder.HasMany(c => c.Books)
            .WithMany(b => b.Categories)
            .UsingEntity(
                "BookCategory",
                l => l.HasOne(typeof(Book)).WithMany().HasForeignKey("BookId").HasConstraintName("FK_BookCategory_Book"),
                r => r.HasOne(typeof(Category)).WithMany().HasForeignKey("CategoryId").HasConstraintName("FK_BookCategory_Category"),
                j =>
                {
                    j.HasKey("BookId", "CategoryId");
                    j.ToTable("BookCategory");
                });
    }
} 
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Represents a book category or genre
/// Demonstrates a many-to-many relationship with Book
/// </summary>
public class Category : BaseEntity<int>
{
    /// <summary>
    /// The name of the category
    /// </summary>
    [Required]
    [MaxLength(100)]
    [Comment("The name of the category")]
    public required string Name { get; set; }
    
    /// <summary>
    /// A description of the category
    /// </summary>
    [MaxLength(500)]
    [Comment("A description of the category")]
    public string? Description { get; set; }
    
    /// <summary>
    /// Many-to-many relationship: A category can contain many books,
    /// and a book can belong to many categories
    /// </summary>
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
} 
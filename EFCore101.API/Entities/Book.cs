using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Represents a book in the library system
/// </summary>
public class Book : BaseEntity<Guid>
{   
    /// <summary>
    /// The title of the book
    /// </summary>
    [Required]
    [MaxLength(255)]
    [Comment("The title of the book")]
    public required string Title { get; set; }

    /// <summary>
    /// The description of the book
    /// </summary>
    [Required]
    [MaxLength(1000)]
    [Comment("The description of the book")]
    public required string Description { get; set; }
    
    /// <summary>
    /// URL to the book's cover image
    /// </summary>
    [Comment("URL to the book's cover image")]
    public required string ImageUrl { get; set; }
    
    /// <summary>
    /// The author who wrote this book (one-to-many relationship)
    /// </summary>
    public Guid? AuthorId { get; set; }
    
    /// <summary>
    /// Navigation property for the author
    /// </summary>
    public virtual Author? Author { get; set; }
    
    /// <summary>
    /// The publisher of this book (one-to-many relationship)
    /// </summary>
    public Guid? PublisherId { get; set; }
    
    /// <summary>
    /// Navigation property for the publisher
    /// </summary>
    public virtual Publisher? Publisher { get; set; }
    
    /// <summary>
    /// One-to-one relationship with BookDetails
    /// </summary>
    public virtual BookDetails? Details { get; set; }
    
    /// <summary>
    /// Many-to-many relationship with Category
    /// </summary>
    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
}
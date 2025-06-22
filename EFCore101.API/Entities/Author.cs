using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Represents an author of books
/// Demonstrates a one-to-many relationship with Book
/// </summary>
public class Author : BaseEntity<Guid>
{
    /// <summary>
    /// The full name of the author
    /// </summary>
    [Required]
    [MaxLength(100)]
    [Comment("The full name of the author")]
    public required string Name { get; set; }
    
    /// <summary>
    /// A short biography of the author
    /// </summary>
    [MaxLength(2000)]
    [Comment("A short biography of the author")]
    public string? Biography { get; set; }
    
    /// <summary>
    /// The author's email address
    /// </summary>
    [MaxLength(150)]
    [Comment("The author's email address")]
    public string? Email { get; set; }
    
    /// <summary>
    /// One-to-many relationship: One author can write many books
    /// </summary>
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
} 
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Represents a book publisher
/// Demonstrates a one-to-many relationship with Book
/// </summary>
public class Publisher : BaseEntity<Guid>
{
    /// <summary>
    /// The name of the publishing company
    /// </summary>
    [Required]
    [MaxLength(200)]
    [Comment("The name of the publishing company")]
    public required string Name { get; set; }
    
    /// <summary>
    /// The publisher's website URL
    /// </summary>
    [MaxLength(255)]
    [Comment("The publisher's website URL")]
    public string? Website { get; set; }
    
    /// <summary>
    /// The publisher's headquarters location
    /// </summary>
    [MaxLength(150)]
    [Comment("The publisher's headquarters location")]
    public string? Location { get; set; }
    
    /// <summary>
    /// One-to-many relationship: One publisher can publish many books
    /// </summary>
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
} 
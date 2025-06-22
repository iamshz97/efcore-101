using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Represents additional details about a book
/// Demonstrates a one-to-one relationship with Book
/// </summary>
public class BookDetails : BaseEntity<Guid>
{
    /// <summary>
    /// Number of pages in the book
    /// </summary>
    [Comment("Number of pages in the book")]
    public int NumberOfPages { get; set; }
    
    /// <summary>
    /// The language the book is written in
    /// </summary>
    [MaxLength(50)]
    [Comment("The language the book is written in")]
    public string? Language { get; set; }
    
    /// <summary>
    /// The ISBN (International Standard Book Number)
    /// </summary>
    [MaxLength(20)]
    [Comment("The ISBN (International Standard Book Number)")]
    public string? ISBN { get; set; }
    
    /// <summary>
    /// The publication date of the book
    /// </summary>
    [Comment("The publication date of the book")]
    public DateTime? PublishedDate { get; set; }
    
    /// <summary>
    /// One-to-one relationship: Each BookDetails belongs to exactly one Book
    /// </summary>
    public Guid BookId { get; set; }
    
    /// <summary>
    /// Navigation property for the one-to-one relationship
    /// </summary>
    public virtual Book Book { get; set; } = null!;
} 
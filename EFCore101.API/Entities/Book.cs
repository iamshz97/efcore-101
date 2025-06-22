using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

public class Book : BaseEntity<Guid>
{   
    public required string Title { get; set; }

    [Comment("The description of the book")]
    [Required]
    [MaxLength(1000)]
    public required string Description { get; set; }
    public required string ImageUrl { get; set; }
}
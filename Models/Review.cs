using System.ComponentModel.DataAnnotations;

namespace Car_Rental_System.Models;

public class Review
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; }

    [Required]
    public int CarId { get; set; }

    [Required, Range(1, 5)]
    public int Rating { get; set; }

    [StringLength(500)]
    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public ApplicationUser User { get; set; }
    public Car Car { get; set; }
}

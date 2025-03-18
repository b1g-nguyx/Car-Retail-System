using System.ComponentModel.DataAnnotations;

namespace Car_Rental_System.Models;
public class Maintenance
{
    public int Id { get; set; }

    [Required]
    public int CarId { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required, StringLength(500)]
    public string? Description { get; set; }
    public Car Car { get; set; }

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; } 
}

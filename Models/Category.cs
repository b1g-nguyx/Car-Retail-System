using System.ComponentModel.DataAnnotations;

namespace Car_Rental_System.Models;
public class Category
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string? Name { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; } = string.Empty;
}



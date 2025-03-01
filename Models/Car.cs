using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Car_Rental_System.Models;
public class Car
{
    [Key]
    public int Id { get; set; }
    public int BrandId { get; set; } // Khóa ngoại đến Brand
    public Brand Brand { get; set; } = null!;

    public int CategoryId { get; set; } // Khóa ngoại đến Category
    public Category Category { get; set; } = null!;

    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PricePerDay { get; set; }
    public bool Availability { get; set; }
    public string Description { get; set; } = string.Empty;
    public string LicensePlates { get; set; } = string.Empty;
    public int Kilometers { get; set; }
    public string FuelType { get; set; } = string.Empty;
    public string Transmission { get; set; } = string.Empty;
    public int Seats { get; set; }
}

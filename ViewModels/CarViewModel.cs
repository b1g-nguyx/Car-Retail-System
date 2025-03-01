using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Car_Rental_System.Models;

public class CarViewModel
{
    [Key]
    public int Id { get; set; }
    public int BrandId { get; set; }
    public int CategoryId { get; set; }

    public Brand Brand { get; set; } = null!;
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
    public List<IFormFile> files { get; set; } = new List<IFormFile>();
    public IEnumerable<Images>? Images { get; set; }
    public IEnumerable<Category>? Categories { get; set; }
    public IEnumerable<Brand>? Brands { get; set; }

}
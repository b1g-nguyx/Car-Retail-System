using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Car_Rental_System.Models.Enums;

namespace Car_Rental_System.Models;

public class Car
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BrandId { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required, StringLength(100)]
    public string Model { get; set; } = string.Empty;

    [Required, Range(1900, 2100)]
    public int Year { get; set; }

    [Required, Range(0, double.MaxValue)]
     [DisplayFormat(DataFormatString = "{0:#,##0} VNĐ", ApplyFormatInEditMode = false)]
    public decimal PricePerDay { get; set; }

    [Required, StringLength(20)]
    public string LicensePlates { get; set; } = string.Empty;

    [Required, Range(0, int.MaxValue)]
    public int Kilometers { get; set; }

    [Required, StringLength(50)]
    public string FuelType { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string Transmission { get; set; } = string.Empty;

    [Required, Range(2, 20)]
    public int Seats { get; set; }

    [Column(TypeName = "nvarchar(MAX)")]
    public string Description { get; set; } = string.Empty;
    public RentalStatus Status { get; set; } = RentalStatus.Available;
    // Thông tin thời gian chỉnh sửa
    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation properties
    public Brand? Brand { get; set; }
    public Category? Category { get; set; }
    public ICollection<Maintenance>? Maintenances { get; set; }
    public ICollection<Review>? Reviews { get; set; }
    public ICollection<CarRental>? CarRentals { get; set; }
}

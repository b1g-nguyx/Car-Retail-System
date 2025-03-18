using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Car_Rental_System.Models.Enums;

namespace Car_Rental_System.Models;

public class CarRental
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int CarId { get; set; }
    [Required]
    public string UserId { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime StartDate { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime EndDate { get; set; }

    [Required, Range(0, double.MaxValue)]
    [DisplayFormat(DataFormatString = "{0:#,##0} VNĐ", ApplyFormatInEditMode = false)]
    public decimal TotalPrice { get; set; }

    [Required]
    public RentalStatus Status { get; set; } = RentalStatus.PendingApproval; // Sử dụng Enum

    public int ContractId { get; set; }
    public string? ImageReturnCarURL { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    // Navigation Properties
    public ApplicationUser User { get; set; }
    public Payment Payment { get; set; }

}


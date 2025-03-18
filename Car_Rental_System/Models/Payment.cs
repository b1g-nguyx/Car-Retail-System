using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Car_Rental_System.Models.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Car_Rental_System.Models;
public class Payment
{
    [Key]
    public int Id { get; set; }
    [Required]
    [ForeignKey(nameof(CarRental))]
    public int RentalId { get; set; }
     [DisplayFormat(DataFormatString = "{0:#,##0} VNĐ", ApplyFormatInEditMode = false)]
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string? PaymentMethod { get; set; }
    public RentalStatus Status { get; set; } = RentalStatus.PendingApproval;
    public CarRental CarRental { get; set; }
}


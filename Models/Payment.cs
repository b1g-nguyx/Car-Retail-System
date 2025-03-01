using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Car_Rental_System.Models;

public class Payment
{
    [Key]
    public int Id { get; set; }


    public int RentalId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }


    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;


    [StringLength(50)]
    public string PaymentMethod { get; set; } = string.Empty;

    [StringLength(50)]
    public string Status { get; set; } = "Chờ xử lý"; // Mặc định là chờ xử lý

    // Khóa ngoại
    [ForeignKey("RentalId")]
    public virtual CarRental? CarRental { get; set; }
}

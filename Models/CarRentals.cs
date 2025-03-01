using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Car_Rental_System.Models;
public class CarRental
{
    [Key]
    public int Id { get; set; }


    public string UserId { get; set; } = string.Empty;


    public int CarId { get; set; }


    public DateTime StartDate { get; set; }


    public DateTime EndDate { get; set; }


    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalPrice { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = "Đang thuê"; // Mặc định là đang thuê

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Khóa ngoại
    [ForeignKey("UserId")]
    public virtual ApplicationUser? User { get; set; }

    [ForeignKey("CarId")]
    public virtual Car? Car { get; set; }
}

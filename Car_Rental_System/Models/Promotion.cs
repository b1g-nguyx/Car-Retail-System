using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Thêm dòng này

namespace Car_Rental_System.Models;
public class Promotion
{
    public int Id { get; set; }

    [Required, StringLength(50)]
    public string Code { get; set; }

    [Required, Range(0, 100)]
     [DisplayFormat(DataFormatString = "{0:#,##0} VNĐ", ApplyFormatInEditMode = false)] // Cấu hình kiểu dữ liệu SQL Server
    public decimal DiscountPercentage { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; } = true;
}

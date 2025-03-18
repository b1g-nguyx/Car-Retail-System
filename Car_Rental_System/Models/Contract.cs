using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Car_Rental_System.Models;
public class Contract
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; }

    [Required, StringLength(100)]
    public string FullName { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required, EmailAddress]
    public string? Email { get; set; }

    [Required, Phone]
    public string? PhoneNumber { get; set; }

    [Required, StringLength(255)]
    public string? Address { get; set; }
    public string? TaxCode { get; set; }

    [Required, Range(0, double.MaxValue)]
    [DisplayFormat(DataFormatString = "{0:#,##0} VNĐ", ApplyFormatInEditMode = false)]
    public decimal TotalPrice { get; set; }

    [Required, Range(0, double.MaxValue)]
    [DisplayFormat(DataFormatString = "{0:#,##0} VNĐ", ApplyFormatInEditMode = false)]
    public decimal Deposit { get; set; }
    public byte[]? PdfFileData { get; set; }
    public string? SignatureImage { get; set; }
    [Required, StringLength(50)]
    public string Status { get; set; } = string.Empty;
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
    public ApplicationUser? User { get; set; }
    [NotMapped]
    public string? PdfBase64 => PdfFileData != null ? Convert.ToBase64String(PdfFileData) : null;

}

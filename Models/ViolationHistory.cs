using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Car_Rental_System.Models;
public class ViolationHistory
{
    public int Id { get; set; }

    [Required]
    public int ContractId { get; set; }

    [Required, StringLength(500)]
    public string? ViolationDetails { get; set; }

    [Required, Range(0, double.MaxValue)]
    [Column(TypeName = "decimal(18,2)")]
    public decimal FineAmount { get; set; }

    [Required]
    public DateTime? ViolationDate { get; set; }

    public Contract Contract { get; set; }
}

using System.ComponentModel.DataAnnotations;
using Car_Rental_System.Models;

namespace Car_Rental_System.ViewModels;
public class PaymentViewModel
{
    public CarViewModel? CarViewModel { get; set; }
    public Profile? Profile { get; set; }
    [DisplayFormat(DataFormatString = "{0:#,##0} VNĐ", ApplyFormatInEditMode = false)]
    public decimal TotalPrice { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? TaxCode { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
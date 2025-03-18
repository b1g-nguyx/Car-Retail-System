using System.ComponentModel.DataAnnotations;
namespace Car_Rental_System.ViewModels;

public class ForgotPasswordViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
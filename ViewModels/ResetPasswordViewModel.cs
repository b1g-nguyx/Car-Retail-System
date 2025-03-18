using System.ComponentModel.DataAnnotations;
namespace Car_Rental_System.ViewModels;

public class ResetPasswordViewModel
{
    [Required]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp.")]
    public string ConfirmPassword { get; set; }

    public string Token { get; set; }
}
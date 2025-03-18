using System.ComponentModel.DataAnnotations;

namespace Car_Rental_System.ViewModels;
public class profileViewModel
{
    public string? UserId { get; set; }
    [Required(ErrorMessage = "Họ và tên không được để trống.")]
    [StringLength(100, ErrorMessage = "Họ và tên không được vượt quá 100 ký tự.")]
    public string? FullName { get; set; }

    [StringLength(255, ErrorMessage = "Địa chỉ không được vượt quá 255 ký tự.")]
    public string? Address { get; set; }

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự.")]
    public string? PhoneNumber { get; set; }
    public string? DrivingLicense { get; set; }
    public string? AvatarUrl { get; set; }
    public IFormFile? AvatarFiles {get; set;}
    public IFormFile? DrivingLicenseFiles {get; set;}
}
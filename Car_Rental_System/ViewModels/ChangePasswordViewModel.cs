using System.ComponentModel.DataAnnotations;

namespace Car_Rental_System.ViewModels
{
    public class ChangePasswordViewModel
    {
        public string UserId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu cũ")]
        [DataType(DataType.Password)]
        public string? OldPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu mới")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}

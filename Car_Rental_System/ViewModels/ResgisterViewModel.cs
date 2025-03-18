using System.ComponentModel.DataAnnotations;
using System;
using Car_Rental_System.Attributes;

namespace Car_Rental_System.ViewModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Họ và tên không được để trống.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Ngày sinh không được để trống.")]
        [DataType(DataType.Date)]
        [MinAge(18, ErrorMessage = "Bạn phải từ 18 tuổi trở lên.")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Giới tính không được để trống.")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string ConfirmPassword { get; set; }
    }
}
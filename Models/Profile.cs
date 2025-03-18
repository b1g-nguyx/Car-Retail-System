using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Car_Rental_System.Models
{


    public class Profile
    {
        [Key]
        public string? UserId { get; set; }


        [Required(ErrorMessage = "Họ và tên không được để trống.")]
        [StringLength(100, ErrorMessage = "Họ và tên không được vượt quá 100 ký tự.")]
        public string FullName { get; set; }

        [StringLength(255, ErrorMessage = "Địa chỉ không được vượt quá 255 ký tự.")]
        public string? Address { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự.")]
        public string? PhoneNumber { get; set; }

        [Url(ErrorMessage = "Giấy phép lái xe không hợp lệ.")]
        public string? DrivingLicense { get; set; }

        [Url(ErrorMessage = "URL ảnh đại diện không hợp lệ.")]
        public string? AvatarUrl { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}

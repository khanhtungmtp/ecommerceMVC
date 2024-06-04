using System.ComponentModel.DataAnnotations;

namespace ECommerceMVC.ViewModels
{
    public class RegisterVM
    {
        [Key]
        [Display(Name ="Tên đăng nhập")]
        [Required(ErrorMessage ="*")]
        [MaxLength(20, ErrorMessage = "Tối đa 20 kí tự")]
        public required string MaKh { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Mật khẩu")]
        [DataType(DataType.Password)]
        public required string MatKhau { get; set; }

        [Display(Name = "Họ tên")]
        [Required(ErrorMessage = "*")]
        [MaxLength(50, ErrorMessage = "Tối đa 50 kí tự")]
        public required string HoTen { get; set; }

        public bool GioiTinh { get; set; } = true;

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime NgaySinh { get; set; }

        [MaxLength(60, ErrorMessage = "Tối đa 60 kí tự")]
        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }

        [Display(Name = "Điện thoại")]
        [MaxLength(11, ErrorMessage = "Tối đa 11 kí tự")]
        [RegularExpression(@"^(09[6-8]|086|03[2-9]|091|094|08[1-5]|088|090|093|07[0-9])\d{7}$", ErrorMessage ="Chưa đúng định dạng di động việt nam")]
        public string? DienThoai { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage ="Chưa đúng định dạng email")]
        public string Email { get; set; } = null!;

        [Display(Name = "Hình")]
        public string? Hinh { get; set; }
    }
}

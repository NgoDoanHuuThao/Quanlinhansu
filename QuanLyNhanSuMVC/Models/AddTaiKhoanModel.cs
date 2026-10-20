using System.ComponentModel.DataAnnotations;

namespace QuanLyNhanSuMVC.Models
{
    public class AddTaiKhoanModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        public string TenDangNhap { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string MatKhau { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng chọn vai trò")]
        public string VaiTro { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng chọn nhân viên")]
        public int NhanVienId { get; set; }
    }
}

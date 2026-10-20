using System.ComponentModel.DataAnnotations;
namespace QuanLyNhanSu.Models.DTO
{
    public class NhanVienRequestDTO
    {
        [Required(ErrorMessage = "Họ tên không được để trống")]
        [MinLength(2, ErrorMessage = "Họ tên phải có ít nhất 2 ký tự")]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "Ngày sinh không được để trống")]
        public DateTime NgaySinh { get; set; }

        [Required(ErrorMessage = "Giới tính không được để trống")]
        public string GioiTinh { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SoDienThoai { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        public string DiaChi { get; set; }

        public DateTime NgayVaoLam { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Lương cơ bản không được âm")]
        public double LuongCoBan { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Phụ cấp không được âm")]
        public double PhuCap { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Thưởng không được âm")]
        public double Thuong { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Phải chọn phòng ban")]
        public int PhongBanId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Phải chọn chức vụ")]
        public int ChucVuId { get; set; }
    }
}
    


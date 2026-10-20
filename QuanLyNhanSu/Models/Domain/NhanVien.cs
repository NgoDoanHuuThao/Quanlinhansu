using System.ComponentModel.DataAnnotations;

namespace QuanLyNhanSu.Models.Domain
{
    public class NhanVien
    {
        public int NhanVienId { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [MinLength(2, ErrorMessage = "Họ tên phải có ít nhất 2 ký tự")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày sinh không được để trống")]
        public DateTime NgaySinh { get; set; }

        [Required(ErrorMessage = "Giới tính không được để trống")]
        public string GioiTinh { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        public string DiaChi { get; set; } = string.Empty;

        public DateTime NgayVaoLam { get; set; }

        [Range(0, double.MaxValue,
            ErrorMessage = "Lương không được âm")]
        public double LuongCoBan { get; set; }

        [Range(0, double.MaxValue,
            ErrorMessage = "Phụ cấp không được âm")]
        public double PhuCap { get; set; }

        [Range(0, double.MaxValue,
            ErrorMessage = "Thưởng không được âm")]
        public double Thuong { get; set; }

        public int PhongBanId { get; set; }

        public int ChucVuId { get; set; }

        // Quan hệ với PhongBan
        public PhongBan? PhongBan { get; set; }

        // Quan hệ với ChucVu
        public ChucVu? ChucVu { get; set; }

        // Quan hệ với TaiKhoan
        public ICollection<TaiKhoan> TaiKhoans { get; set; }
            = new List<TaiKhoan>();

        // Quan hệ với ChamCong
        public ICollection<ChamCong> ChamCongs { get; set; }
            = new List<ChamCong>();

        // Quan hệ với BangLuong
        public ICollection<BangLuong> BangLuongs { get; set; }
            = new List<BangLuong>();
    }
}
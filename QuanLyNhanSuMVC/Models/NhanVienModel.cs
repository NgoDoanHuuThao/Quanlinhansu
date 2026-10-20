namespace QuanLyNhanSuMVC.Models
{
    public class NhanVienModel
    {
        public int NhanVienId { get; set; }

        public string HoTen { get; set; } = "";

        public DateTime? NgaySinh { get; set; }

        public string? GioiTinh { get; set; }

        public string? SoDienThoai { get; set; }

        public string? Email { get; set; }

        public string? DiaChi { get; set; }

        public DateTime? NgayVaoLam { get; set; }

        public double? LuongCoBan { get; set; }

        public double? PhuCap { get; set; }

        public double? Thuong { get; set; }

        // Không nullable vì API yêu cầu int
        public int PhongBanId { get; set; }

        public string? TenPhongBan { get; set; }

        // Không nullable vì API yêu cầu int
        public int ChucVuId { get; set; }

        public string? TenChucVu { get; set; }
    }
}

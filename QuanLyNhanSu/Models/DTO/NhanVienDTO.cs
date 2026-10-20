namespace QuanLyNhanSu.Models.DTO
{
    public class NhanVienDTO
    {
        public int NhanVienId { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public DateTime? NgaySinh { get; set; }
        public string? GioiTinh { get; set; }
        public string? SoDienThoai { get; set; }
        public string? Email { get; set; }
        public string? DiaChi { get; set; }
        public DateTime? NgayVaoLam { get; set; }

        public double? LuongCoBan { get; set; }
        public double? PhuCap { get; set; }
        public double? Thuong { get; set; }

        public int PhongBanId { get; set; }
        public string? TenPhongBan { get; set; }

        public int ChucVuId { get; set; }
        public string? TenChucVu { get; set; }
    }
}

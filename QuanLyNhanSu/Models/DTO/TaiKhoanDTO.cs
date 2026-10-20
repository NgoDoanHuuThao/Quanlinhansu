namespace QuanLyNhanSu.Models.DTO
{
    public class TaiKhoanDTO
    {
        public int TaiKhoanId { get; set; }
        public string TenDangNhap { get; set; } = string.Empty;
        public string VaiTro { get; set; } = string.Empty;
        public int? NhanVienId { get; set; }
        public string? HoTenNhanVien { get; set; }
    }
}

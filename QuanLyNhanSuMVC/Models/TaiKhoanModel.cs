namespace QuanLyNhanSuMVC.Models
{
    public class TaiKhoanModel
    {
        public int TaiKhoanId { get; set; }

        public string TenDangNhap { get; set; } = "";

        public string VaiTro { get; set; } = "";

        public int? NhanVienId { get; set; }

        public string? HoTenNhanVien { get; set; }
    }
}

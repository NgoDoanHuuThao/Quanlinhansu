namespace QuanLyNhanSu.Models.Domain
{
    public class TaiKhoan
    {
        public int TaiKhoanId { get; set; }

        public string TenDangNhap { get; set; } = string.Empty;

        public string MatKhau { get; set; } = string.Empty;

        public string VaiTro { get; set; } = string.Empty;

        public int? NhanVienId { get; set; }

        public NhanVien? NhanVien { get; set; }
    }
}

namespace QuanLyNhanSu.Models.DTO
{
    public class AddTaiKhoanRequestDTO
    {
        public string TenDangNhap { get; set; } = string.Empty;

        public string MatKhau { get; set; } = string.Empty;

        public string VaiTro { get; set; } = string.Empty;

        public int NhanVienId { get; set; }
    }
}

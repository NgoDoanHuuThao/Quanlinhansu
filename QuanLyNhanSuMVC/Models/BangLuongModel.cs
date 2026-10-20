namespace QuanLyNhanSuMVC.Models
{
    public class BangLuongModel
    {
        public int BangLuongId { get; set; }

        public int NhanVienId { get; set; }

        public string? HoTenNhanVien { get; set; }

        public int Thang { get; set; }

        public int Nam { get; set; }

        public double? LuongCoBan { get; set; }

        public double? PhuCap { get; set; }

        public double? Thuong { get; set; }

        public double? TongLuong { get; set; }
        public int SoNgayNghiKhongLuong { get; set; }

        public double? TienTruNghi { get; set; }
    }
}

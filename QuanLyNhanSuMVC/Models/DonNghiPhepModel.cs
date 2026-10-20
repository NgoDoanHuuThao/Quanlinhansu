using System.ComponentModel.DataAnnotations;

namespace QuanLyNhanSuMVC.Models
{
    public class DonNghiPhepModel
    {
        public int DonNghiPhepId { get; set; }
        public int NhanVienId { get; set; }

        public string? HoTenNhanVien { get; set; }
        public string? TenPhongBan { get; set; }

        public DateTime NgayVietDon { get; set; }
        public DateTime NgayBatDauNghi { get; set; }
        public DateTime NgayKetThucNghi { get; set; }

        public int SoNgayNghi { get; set; }
        public string LyDoNghi { get; set; } = string.Empty;

        public string TrangThai { get; set; } = "ChoDuyet";

        public int? NguoiDuyetId { get; set; }
        public string? HoTenNguoiDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public string? GhiChuDuyet { get; set; }
        public bool HuongLuong { get; set; } = true;
    }
}

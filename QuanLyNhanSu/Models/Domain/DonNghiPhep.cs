using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhanSu.Models.Domain
{
    [Table("DonNghiPheps")]
    public class DonNghiPhep
    {
        [Key]
        public int DonNghiPhepId { get; set; }

        [Required]
        public int NhanVienId { get; set; }

        public DateTime NgayVietDon { get; set; }

        public DateTime NgayBatDauNghi { get; set; }

        public DateTime NgayKetThucNghi { get; set; }

        public int SoNgayNghi { get; set; }
        public bool HuongLuong { get; set; } = true;

        [Required]
        [MaxLength(500)]
        public string LyDoNghi { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string TrangThai { get; set; } = "ChoDuyet";

        public int? NguoiDuyetId { get; set; }

        public DateTime? NgayDuyet { get; set; }

        [MaxLength(500)]
        public string? GhiChuDuyet { get; set; }

        [ForeignKey(nameof(NhanVienId))]
        public NhanVien? NhanVien { get; set; }

        [ForeignKey(nameof(NguoiDuyetId))]
        public NhanVien? NguoiDuyet { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
namespace QuanLyNhanSuMVC.Models
{
    public class ThongBaoModel
    {
        public int ThongBaoId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
        [StringLength(200)]
        public string TieuDe { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập nội dung")]
        public string NoiDung { get; set; } = string.Empty;

        public string DoiTuongNhan { get; set; } = "ALL";

        public int? PhongBanId { get; set; }

        public int NguoiGuiId { get; set; }

        public DateTime NgayTao { get; set; }

        public DateTime? NgayHetHan { get; set; }

        public bool TrangThai { get; set; }

        public bool DaDoc { get; set; }
    }
}

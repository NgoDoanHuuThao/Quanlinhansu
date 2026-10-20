using System.ComponentModel.DataAnnotations;
namespace QuanLyNhanSu.Models.Domain
{
    public class ThongBao
    {
        public int ThongBaoId { get; set; }

        [Required]
        [StringLength(200)]
        public string TieuDe { get; set; } = string.Empty;

        [Required]
        public string NoiDung { get; set; } = string.Empty;

        public string DoiTuongNhan { get; set; } = "ALL";

        public int? PhongBanId { get; set; }

        public int NguoiGuiId { get; set; }

        public DateTime NgayTao { get; set; }

        public DateTime? NgayHetHan { get; set; }

        public bool TrangThai { get; set; } = true;

        public ICollection<ThongBaoNguoiNhan> NguoiNhans { get; set; }
            = new List<ThongBaoNguoiNhan>();
    }
}

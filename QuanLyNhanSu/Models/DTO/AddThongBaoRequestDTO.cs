using System.ComponentModel.DataAnnotations;

namespace QuanLyNhanSu.Models.DTO
{
    public class AddThongBaoRequestDTO
    {
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
        [StringLength(200)]
        public string TieuDe { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập nội dung")]
        public string NoiDung { get; set; } = string.Empty;

        // ALL hoặc PHONGBAN
        public string DoiTuongNhan { get; set; } = "ALL";

        public int? PhongBanId { get; set; }

        public DateTime? NgayHetHan { get; set; }
    }
}

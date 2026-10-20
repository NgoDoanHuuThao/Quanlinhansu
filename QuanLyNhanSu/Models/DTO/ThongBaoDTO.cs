namespace QuanLyNhanSu.Models.DTO
{
    public class ThongBaoDTO
    {
        public int ThongBaoId { get; set; }

        public string TieuDe { get; set; } = string.Empty;

        public string NoiDung { get; set; } = string.Empty;

        // ALL: tất cả nhân viên
        // PHONGBAN: nhân viên thuộc phòng ban
        public string DoiTuongNhan { get; set; } = "ALL";

        public int? PhongBanId { get; set; }

        public int NguoiGuiId { get; set; }

        public DateTime NgayTao { get; set; }

        public DateTime? NgayHetHan { get; set; }

        public bool TrangThai { get; set; }

        public bool DaDoc { get; set; }
    }
}

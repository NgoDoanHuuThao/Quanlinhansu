namespace QuanLyNhanSu.Models.Domain
{
    public class PhongBan
    {
        public int PhongBanId { get; set; }

        public string TenPhongBan { get; set; } = string.Empty;

        public string? MoTa { get; set; }

        public List<NhanVien> NhanViens { get; set; }
            = new List<NhanVien>();
    }
}

namespace QuanLyNhanSu.Models.Domain
{
    public class ChucVu
    {
        public int ChucVuId { get; set; }

        public string TenChucVu { get; set; } = string.Empty;

        public string? MoTa { get; set; }

        public List<NhanVien> NhanViens { get; set; }
            = new List<NhanVien>();
    }
}

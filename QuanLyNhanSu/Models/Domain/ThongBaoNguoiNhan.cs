namespace QuanLyNhanSu.Models.Domain
{
    public class ThongBaoNguoiNhan
    {
        public int ThongBaoNguoiNhanId { get; set; }

        public int ThongBaoId { get; set; }

        public int NhanVienId { get; set; }

        public bool DaDoc { get; set; } = false;

        public DateTime? NgayDoc { get; set; }

        public ThongBao? ThongBao { get; set; }

        public NhanVien? NhanVien { get; set; }
    }
}

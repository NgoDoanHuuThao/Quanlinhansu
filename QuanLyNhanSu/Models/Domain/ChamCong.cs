namespace QuanLyNhanSu.Models.Domain
{
    public class ChamCong
    {
        public int ChamCongId { get; set; }

        public int NhanVienId { get; set; }

        public NhanVien? NhanVien { get; set; }

        public DateTime ThoiGianCham { get; set; }

        public string? HinhAnh { get; set; }

        public string? ViTri { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}

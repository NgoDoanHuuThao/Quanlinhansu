using System.Text.Json.Serialization;

namespace QuanLyNhanSuMVC.Models
{
    public class LichSuChamCongModel
    {
        public int ChamCongId { get; set; }

        public int NhanVienId { get; set; }

        public string? HoTenNhanVien { get; set; }

        // API trả về "thoiGianCham"
        // MVC nhận vào "ThoiGianChamCong"
        [JsonPropertyName("thoiGianCham")]
        public DateTime ThoiGianChamCong { get; set; }

        public string? HinhAnh { get; set; }

        public string? ViTri { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}

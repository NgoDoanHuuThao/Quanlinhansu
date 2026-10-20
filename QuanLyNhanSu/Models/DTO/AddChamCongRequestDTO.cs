using System.ComponentModel.DataAnnotations;

namespace QuanLyNhanSu.Models.DTO
{
    public class AddChamCongRequestDTO
    {
        [Required(ErrorMessage = "Phải chọn ảnh chấm công")]
        public IFormFile? HinhAnh { get; set; }

        [Required(ErrorMessage = "Vị trí không được để trống")]
        public string ViTri { get; set; } = string.Empty;

        [Required(ErrorMessage = "Latitude không được để trống")]
        public string Latitude { get; set; } = string.Empty;

        [Required(ErrorMessage = "Longitude không được để trống")]
        public string Longitude { get; set; } = string.Empty;
    }
}
using System.ComponentModel.DataAnnotations;
namespace QuanLyNhanSuMVC.Models
{
    public class DuyetDonNghiPhepModel
    {
        [Required]
        public string TrangThai { get; set; } = "DaDuyet";

        [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự.")]
        public string? GhiChuDuyet { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
namespace QuanLyNhanSu.Models.DTO
{
    public class TaoBangLuongRequestDTO
    {
        [Range(1, 12, ErrorMessage = "Tháng phải từ 1 đến 12")]
        public int Thang { get; set; }

        [Range(2000, 2100, ErrorMessage = "Năm không hợp lệ")]
        public int Nam { get; set; }
    }
}

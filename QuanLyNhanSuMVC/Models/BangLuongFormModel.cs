using System.ComponentModel.DataAnnotations;

namespace QuanLyNhanSuMVC.Models
{
    public class BangLuongFormModel
    {
        [Required(ErrorMessage = "Vui lòng chọn nhân viên")]
        [Range(1, int.MaxValue,
            ErrorMessage = "Vui lòng chọn nhân viên")]
        public int NhanVienId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tháng")]
        [Range(1, 12,
            ErrorMessage = "Tháng phải từ 1 đến 12")]
        public int Thang { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập năm")]
        [Range(2000, 2100,
            ErrorMessage = "Năm không hợp lệ")]
        public int Nam { get; set; }

        public double? LuongCoBan { get; set; }

        public double? PhuCap { get; set; }

        public double? Thuong { get; set; }
    }
}

using System.Collections.Generic;
namespace QuanLyNhanSuMVC.Models
{
    public class DashboardModel
    {
        public int TongNhanVien { get; set; }

        public int TongTaiKhoan { get; set; }

        public double TongQuyLuong { get; set; }

        public List<string> ThangLabels { get; set; } = new List<string>();

        public List<double> QuyLuongData { get; set; } = new List<double>();
    }
}

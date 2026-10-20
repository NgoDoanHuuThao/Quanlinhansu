using QuanLyNhanSu.Models.DTO;

namespace QuanLyNhanSu.Repositories
{
    public interface IBangLuongRepository
    {
        List<BangLuongDTO> GetAllBangLuongs();

        BangLuongDTO? GetBangLuongById(int id);

        BangLuongDTO AddBangLuong(BangLuongDTO bangLuongDTO);

        BangLuongDTO? UpdateBangLuongById(
            int id,
            BangLuongDTO bangLuongDTO);

        bool DeleteBangLuongById(int id);
        List<BangLuongDTO> TaoBangLuongChoTatCaNhanVien(
    int thang,
    int nam);
        List<BangLuongDTO> GetBangLuongCuaNhanVien(int nhanVienId);

        List<BangLuongDTO> GetBangLuongTheoThangNam(
        int thang,
        int nam);

        ThongKeLuongDTO GetThongKeLuong(
            int thang,
            int nam);

        List<ThongKeLuongNamDTO> GetThongKeLuongTheoNam(
        int nam);
    }
}

using QuanLyNhanSu.Models.DTO;
namespace QuanLyNhanSu.Repositories
{
    public interface IThongBaoRepository
    {
        List<ThongBaoDTO> GetThongBaoCuaNhanVien(int nhanVienId);

        List<ThongBaoDTO> GetAllThongBao();

        ThongBaoDTO? GetThongBaoById(int id);

        ThongBaoDTO AddThongBao(
            AddThongBaoRequestDTO request,
            int nguoiGuiId);

        bool CapNhatDaDoc(int thongBaoId, int nhanVienId);

        bool DeleteThongBao(int id);
    }
}

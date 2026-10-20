using QuanLyNhanSu.Models.DTO;

namespace QuanLyNhanSu.Repositories
{
    public interface IChamCongRepository
    {
        List<ChamCongDTO> GetAllChamCongs();

        ChamCongDTO? GetChamCongById(int id);

        ChamCongDTO AddChamCong(ChamCongDTO chamCongDTO);

        Task<ChamCongDTO> AddChamCong(
    int nhanVienId,
    AddChamCongRequestDTO request);

        ChamCongDTO? UpdateChamCongById(
            int id,
            ChamCongDTO chamCongDTO);

        bool DeleteChamCongById(int id);
    }
}

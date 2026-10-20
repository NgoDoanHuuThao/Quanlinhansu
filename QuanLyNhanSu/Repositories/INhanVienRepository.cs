using QuanLyNhanSu.Models.DTO;

namespace QuanLyNhanSu.Repositories
{
    public interface INhanVienRepository
    {
        List<NhanVienDTO> GetAllNhanViens(
            string? filterOn = null,
            string? filterQuery = null,
            string? sortBy = null,
            bool isAscending = true,
            int pageNumber = 1,
            int pageSize = 10);

        NhanVienDTO? GetNhanVienById(int id);

        NhanVienDTO AddNhanVien(
            AddNhanVienRequestDTO request);

        NhanVienDTO? UpdateNhanVienById(
            int id,
            AddNhanVienRequestDTO request);

        bool DeleteNhanVienById(int id);
    }
}
using QuanLyNhanSu.Models.Domain;
namespace QuanLyNhanSu.Repositories
{
    public interface IDonNghiPhepRepository
    {
        Task<List<DonNghiPhep>> GetAllAsync();

        Task<List<DonNghiPhep>> GetByNhanVienIdAsync(
            int nhanVienId);

        Task<DonNghiPhep?> GetByIdAsync(int id);

        Task<DonNghiPhep> CreateAsync(DonNghiPhep don);

        Task<bool> UpdateAsync(DonNghiPhep don);
    }
}

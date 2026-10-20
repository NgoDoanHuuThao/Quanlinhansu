using QuanLyNhanSu.Models.DTO;

namespace QuanLyNhanSu.Repositories
{
    public interface IChucVuRepository
    {
        List<ChucVuDTO> GetAllChucVus();

        ChucVuDTO? GetChucVuById(int id);

        ChucVuDTO AddChucVu(ChucVuDTO chucVuDTO);

        ChucVuDTO? UpdateChucVuById(
            int id,
            ChucVuDTO chucVuDTO);

        bool DeleteChucVuById(int id);
    }
}

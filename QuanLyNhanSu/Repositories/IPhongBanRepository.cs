using QuanLyNhanSu.Models.DTO;

namespace QuanLyNhanSu.Repositories
{
    public interface IPhongBanRepository
    {
        List<PhongBanDTO> GetAllPhongBans();

        PhongBanDTO? GetPhongBanById(int id);

        PhongBanDTO AddPhongBan(PhongBanDTO phongBanDTO);

        PhongBanDTO? UpdatePhongBanById(
            int id,
            PhongBanDTO phongBanDTO);

        bool DeletePhongBanById(int id);
    }
}

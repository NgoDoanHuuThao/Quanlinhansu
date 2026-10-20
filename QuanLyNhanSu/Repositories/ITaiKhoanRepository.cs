using QuanLyNhanSu.Models.DTO;
namespace QuanLyNhanSu.Repositories
{
    public interface ITaiKhoanRepository
    {
        // GET ALL
        List<TaiKhoanDTO> GetAllTaiKhoans();

        // GET BY ID
        TaiKhoanDTO? GetTaiKhoanById(int id);

        // ADD
        TaiKhoanDTO AddTaiKhoan(
            AddTaiKhoanRequestDTO request);

        // UPDATE
        TaiKhoanDTO? UpdateTaiKhoanById(
            int id,
            TaiKhoanDTO taiKhoanDTO);

        // DELETE
        bool DeleteTaiKhoanById(int id);

        // LOGIN
        TaiKhoanDTO? Login(
            LoginRequestDTO request);

        // KIỂM TRA TÊN ĐĂNG NHẬP
        bool IsTenDangNhapTonTai(
            string tenDangNhap);

        // KIỂM TRA NHÂN VIÊN ĐÃ CÓ TÀI KHOẢN
        bool IsNhanVienDaCoTaiKhoan(
            int nhanVienId);
    }
}

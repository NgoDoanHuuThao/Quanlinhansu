using QuanLyNhanSu.Data;
using QuanLyNhanSu.Models.Domain;
using QuanLyNhanSu.Models.DTO;
namespace QuanLyNhanSu.Repositories
{
    public class SQLTaiKhoanRepository : ITaiKhoanRepository
    {
        private readonly AppDbContext _context;

        public SQLTaiKhoanRepository(AppDbContext context)
        {
            _context = context;
        }

        // GET ALL
        public List<TaiKhoanDTO> GetAllTaiKhoans()
        {
            return _context.TaiKhoans
                .Select(t => new TaiKhoanDTO
                {
                    TaiKhoanId = t.TaiKhoanId,
                    TenDangNhap = t.TenDangNhap,
                    VaiTro = t.VaiTro,
                    NhanVienId = t.NhanVienId,
                    HoTenNhanVien = t.NhanVien != null
                        ? t.NhanVien.HoTen
                        : null
                })
                .ToList();
        }

        // GET BY ID
        public TaiKhoanDTO? GetTaiKhoanById(int id)
        {
            return _context.TaiKhoans
                .Where(t => t.TaiKhoanId == id)
                .Select(t => new TaiKhoanDTO
                {
                    TaiKhoanId = t.TaiKhoanId,
                    TenDangNhap = t.TenDangNhap,
                    VaiTro = t.VaiTro,
                    NhanVienId = t.NhanVienId,
                    HoTenNhanVien = t.NhanVien != null
                        ? t.NhanVien.HoTen
                        : null
                })
                .FirstOrDefault();
        }

        // ADD
        public TaiKhoanDTO AddTaiKhoan(
    AddTaiKhoanRequestDTO request)
        {
            var taiKhoan = new TaiKhoan
            {
                TenDangNhap = request.TenDangNhap,
                MatKhau = request.MatKhau,
                VaiTro = request.VaiTro,
                NhanVienId = request.NhanVienId
            };

            _context.TaiKhoans.Add(taiKhoan);

            _context.SaveChanges();

            return GetTaiKhoanById(
                taiKhoan.TaiKhoanId)!;
        }
        public bool IsTenDangNhapTonTai(
    string tenDangNhap)
        {
            return _context.TaiKhoans
                .Any(t =>
                    t.TenDangNhap == tenDangNhap);
        }
        public bool IsNhanVienDaCoTaiKhoan(
    int nhanVienId)
        {
            return _context.TaiKhoans
                .Any(t =>
                    t.NhanVienId == nhanVienId);
        }

        // UPDATE
        public TaiKhoanDTO? UpdateTaiKhoanById(
            int id,
            TaiKhoanDTO taiKhoanDTO)
        {
            var taiKhoan = _context.TaiKhoans
                .FirstOrDefault(t => t.TaiKhoanId == id);

            if (taiKhoan == null)
            {
                return null;
            }

            taiKhoan.TenDangNhap =
                taiKhoanDTO.TenDangNhap;

            taiKhoan.VaiTro =
                taiKhoanDTO.VaiTro;

            taiKhoan.NhanVienId =
                taiKhoanDTO.NhanVienId;

            _context.SaveChanges();

            return GetTaiKhoanById(id);
        }

        // DELETE
        public bool DeleteTaiKhoanById(int id)
        {
            var taiKhoan = _context.TaiKhoans
                .FirstOrDefault(t => t.TaiKhoanId == id);

            if (taiKhoan == null)
            {
                return false;
            }

            _context.TaiKhoans.Remove(taiKhoan);
            _context.SaveChanges();

            return true;
        }

        // LOGIN
        public TaiKhoanDTO? Login(
            LoginRequestDTO request)
        {
            return _context.TaiKhoans
                .Where(t =>
                    t.TenDangNhap == request.TenDangNhap &&
                    t.MatKhau == request.MatKhau)
                .Select(t => new TaiKhoanDTO
                {
                    TaiKhoanId = t.TaiKhoanId,
                    TenDangNhap = t.TenDangNhap,
                    VaiTro = t.VaiTro,
                    NhanVienId = t.NhanVienId,
                    HoTenNhanVien = t.NhanVien != null
                        ? t.NhanVien.HoTen
                        : null
                })
                .FirstOrDefault();
        }
    }
}

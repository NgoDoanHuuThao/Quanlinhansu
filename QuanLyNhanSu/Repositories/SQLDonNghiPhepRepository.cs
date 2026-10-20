using Microsoft.EntityFrameworkCore;
using QuanLyNhanSu.Data;
using QuanLyNhanSu.Models.Domain;

namespace QuanLyNhanSu.Repositories
{
    public class SQLDonNghiPhepRepository : IDonNghiPhepRepository
    {
        private readonly AppDbContext _context;

        public SQLDonNghiPhepRepository(AppDbContext context)
        {
            _context = context;
        }

        // Lấy tất cả đơn nghỉ phép, kèm nhân viên, phòng ban và người duyệt
        public async Task<List<DonNghiPhep>> GetAllAsync()
        {
            return await _context.DonNghiPheps
                .Include(x => x.NhanVien)
                    .ThenInclude(nv => nv.PhongBan)
                .Include(x => x.NguoiDuyet)
                .OrderByDescending(x => x.NgayVietDon)
                .ToListAsync();
        }

        // Lấy đơn nghỉ phép của một nhân viên
        public async Task<List<DonNghiPhep>> GetByNhanVienIdAsync(
            int nhanVienId)
        {
            return await _context.DonNghiPheps
                .Include(x => x.NhanVien)
                    .ThenInclude(nv => nv.PhongBan)
                .Include(x => x.NguoiDuyet)
                .Where(x => x.NhanVienId == nhanVienId)
                .OrderByDescending(x => x.NgayVietDon)
                .ToListAsync();
        }

        // Lấy chi tiết một đơn nghỉ phép
        public async Task<DonNghiPhep?> GetByIdAsync(int id)
        {
            return await _context.DonNghiPheps
                .Include(x => x.NhanVien)
                    .ThenInclude(nv => nv.PhongBan)
                .Include(x => x.NguoiDuyet)
                .FirstOrDefaultAsync(
                    x => x.DonNghiPhepId == id);
        }

        // Tạo đơn nghỉ phép
        public async Task<DonNghiPhep> CreateAsync(
            DonNghiPhep don)
        {
            _context.DonNghiPheps.Add(don);

            await _context.SaveChangesAsync();

            return don;
        }

        // Cập nhật đơn nghỉ phép
        public async Task<bool> UpdateAsync(
            DonNghiPhep don)
        {
            _context.DonNghiPheps.Update(don);

            return await _context.SaveChangesAsync() > 0;
        }
    }
}

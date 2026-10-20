using Microsoft.EntityFrameworkCore;
using QuanLyNhanSu.Data;
using QuanLyNhanSu.Models.Domain;
using QuanLyNhanSu.Models.DTO;
namespace QuanLyNhanSu.Repositories
{
    public class SQLNhanVienRepository : INhanVienRepository
    {
        private readonly AppDbContext _context;

        public SQLNhanVienRepository(AppDbContext context)
        {
            _context = context;
        }

     
        public List<NhanVienDTO> GetAllNhanViens(
            string? filterOn = null,
            string? filterQuery = null,
            string? sortBy = null,
            bool isAscending = true,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _context.NhanViens
                .Include(n => n.PhongBan)
                .Include(n => n.ChucVu)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filterQuery))
            {
                filterQuery = filterQuery.Trim();

                if (string.IsNullOrWhiteSpace(filterOn) ||
                    filterOn.Equals("HoTen", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(n =>
                        n.HoTen.Contains(filterQuery));
                }
                else if (filterOn.Equals(
                    "SoDienThoai",
                    StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(n =>
                        n.SoDienThoai.Contains(filterQuery));
                }
                else if (filterOn.Equals(
                    "Email",
                    StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(n =>
                        n.Email.Contains(filterQuery));
                }
                else if (filterOn.Equals(
                    "GioiTinh",
                    StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(n =>
                        n.GioiTinh.Contains(filterQuery));
                }
                else if (filterOn.Equals(
                    "DiaChi",
                    StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(n =>
                        n.DiaChi.Contains(filterQuery));
                }
                else if (filterOn.Equals(
                    "TenPhongBan",
                    StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(n =>
                        n.PhongBan != null &&
                        n.PhongBan.TenPhongBan.Contains(filterQuery));
                }
                else if (filterOn.Equals(
                    "TenChucVu",
                    StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(n =>
                        n.ChucVu != null &&
                        n.ChucVu.TenChucVu.Contains(filterQuery));
                }
            }

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                if (sortBy.Equals(
                    "HoTen",
                    StringComparison.OrdinalIgnoreCase))
                {
                    query = isAscending
                        ? query.OrderBy(n => n.HoTen)
                        : query.OrderByDescending(n => n.HoTen);
                }
                else if (sortBy.Equals(
                    "NgaySinh",
                    StringComparison.OrdinalIgnoreCase))
                {
                    query = isAscending
                        ? query.OrderBy(n => n.NgaySinh)
                        : query.OrderByDescending(n => n.NgaySinh);
                }
                else if (sortBy.Equals(
                    "NgayVaoLam",
                    StringComparison.OrdinalIgnoreCase))
                {
                    query = isAscending
                        ? query.OrderBy(n => n.NgayVaoLam)
                        : query.OrderByDescending(n => n.NgayVaoLam);
                }
                else if (sortBy.Equals(
                    "LuongCoBan",
                    StringComparison.OrdinalIgnoreCase))
                {
                    query = isAscending
                        ? query.OrderBy(n => n.LuongCoBan)
                        : query.OrderByDescending(n => n.LuongCoBan);
                }
                else if (sortBy.Equals(
                    "PhuCap",
                    StringComparison.OrdinalIgnoreCase))
                {
                    query = isAscending
                        ? query.OrderBy(n => n.PhuCap)
                        : query.OrderByDescending(n => n.PhuCap);
                }
                else if (sortBy.Equals(
                    "Thuong",
                    StringComparison.OrdinalIgnoreCase))
                {
                    query = isAscending
                        ? query.OrderBy(n => n.Thuong)
                        : query.OrderByDescending(n => n.Thuong);
                }
            }
            else
            {
                query = query.OrderBy(n => n.NhanVienId);
            }


            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 10;
            }

            if (pageSize > 100)
            {
                pageSize = 100;
            }

            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);


            return query
                .Select(n => new NhanVienDTO
                {
                    NhanVienId = n.NhanVienId,
                    HoTen = n.HoTen,
                    NgaySinh = n.NgaySinh,
                    GioiTinh = n.GioiTinh,
                    SoDienThoai = n.SoDienThoai,
                    Email = n.Email,
                    DiaChi = n.DiaChi,
                    NgayVaoLam = n.NgayVaoLam,
                    LuongCoBan = n.LuongCoBan,
                    PhuCap = n.PhuCap,
                    Thuong = n.Thuong,

                    PhongBanId = n.PhongBanId,
                    TenPhongBan = n.PhongBan != null
                        ? n.PhongBan.TenPhongBan
                        : null,

                    ChucVuId = n.ChucVuId,
                    TenChucVu = n.ChucVu != null
                        ? n.ChucVu.TenChucVu
                        : null
                })
                .ToList();
        }

        // GET BY ID
        public NhanVienDTO? GetNhanVienById(int id)
        {
            return _context.NhanViens
                .Include(n => n.PhongBan)
                .Include(n => n.ChucVu)
                .Where(n => n.NhanVienId == id)
                .Select(n => new NhanVienDTO
                {
                    NhanVienId = n.NhanVienId,
                    HoTen = n.HoTen,
                    NgaySinh = n.NgaySinh,
                    GioiTinh = n.GioiTinh,
                    SoDienThoai = n.SoDienThoai,
                    Email = n.Email,
                    DiaChi = n.DiaChi,
                    NgayVaoLam = n.NgayVaoLam,
                    LuongCoBan = n.LuongCoBan,
                    PhuCap = n.PhuCap,
                    Thuong = n.Thuong,

                    PhongBanId = n.PhongBanId,
                    TenPhongBan = n.PhongBan != null
                        ? n.PhongBan.TenPhongBan
                        : null,

                    ChucVuId = n.ChucVuId,
                    TenChucVu = n.ChucVu != null
                        ? n.ChucVu.TenChucVu
                        : null
                })
                .FirstOrDefault();
        }

        // ADD
        public NhanVienDTO AddNhanVien(
            AddNhanVienRequestDTO request)
        {
            var nhanVien = new NhanVien
            {
                HoTen = request.HoTen,
                NgaySinh = request.NgaySinh,
                GioiTinh = request.GioiTinh,
                SoDienThoai = request.SoDienThoai,
                Email = request.Email,
                DiaChi = request.DiaChi,
                NgayVaoLam = request.NgayVaoLam,
                LuongCoBan = request.LuongCoBan,
                PhuCap = request.PhuCap,
                Thuong = request.Thuong,
                PhongBanId = request.PhongBanId,
                ChucVuId = request.ChucVuId
            };

            _context.NhanViens.Add(nhanVien);
            _context.SaveChanges();

            return GetNhanVienById(nhanVien.NhanVienId)!;
        }

        // UPDATE
        public NhanVienDTO? UpdateNhanVienById(
            int id,
            AddNhanVienRequestDTO request)
        {
            var nhanVien = _context.NhanViens
                .FirstOrDefault(n => n.NhanVienId == id);

            if (nhanVien == null)
            {
                return null;
            }

            nhanVien.HoTen = request.HoTen;
            nhanVien.NgaySinh = request.NgaySinh;
            nhanVien.GioiTinh = request.GioiTinh;
            nhanVien.SoDienThoai = request.SoDienThoai;
            nhanVien.Email = request.Email;
            nhanVien.DiaChi = request.DiaChi;
            nhanVien.NgayVaoLam = request.NgayVaoLam;
            nhanVien.LuongCoBan = request.LuongCoBan;
            nhanVien.PhuCap = request.PhuCap;
            nhanVien.Thuong = request.Thuong;
            nhanVien.PhongBanId = request.PhongBanId;
            nhanVien.ChucVuId = request.ChucVuId;

            _context.SaveChanges();

            return GetNhanVienById(id);
        }

        // DELETE
        public bool DeleteNhanVienById(int id)
        {
            var nhanVien = _context.NhanViens
                .FirstOrDefault(n => n.NhanVienId == id);

            if (nhanVien == null)
            {
                return false;
            }

            _context.NhanViens.Remove(nhanVien);
            _context.SaveChanges();

            return true;
        }
    }
}
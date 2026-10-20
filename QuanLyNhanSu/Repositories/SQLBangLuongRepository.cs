using QuanLyNhanSu.Data;
using QuanLyNhanSu.Models.Domain;
using QuanLyNhanSu.Models.DTO;
namespace QuanLyNhanSu.Repositories
{
    public class SQLBangLuongRepository : IBangLuongRepository
    {
        private readonly AppDbContext _context;

        public SQLBangLuongRepository(AppDbContext context)
        {
            _context = context;
        }

        // GET ALL
        public List<BangLuongDTO> GetAllBangLuongs()
        {
            return _context.BangLuongs
                .Select(b => new BangLuongDTO
                {
                    BangLuongId = b.BangLuongId,
                    NhanVienId = b.NhanVienId,

                    HoTenNhanVien = b.NhanVien != null
                        ? b.NhanVien.HoTen
                        : null,

                    Thang = b.Thang,
                    Nam = b.Nam,

                    LuongCoBan = b.LuongCoBan,
                    PhuCap = b.PhuCap,
                    Thuong = b.Thuong,
                    SoNgayNghiKhongLuong = b.SoNgayNghiKhongLuong,
                    TienTruNghi = b.TienTruNghi,
                    TongLuong = b.TongLuong
                })
                .ToList();
        }

        // GET BY ID
        public BangLuongDTO? GetBangLuongById(int id)
        {
            return _context.BangLuongs
                .Where(b => b.BangLuongId == id)
                .Select(b => new BangLuongDTO
                {
                    BangLuongId = b.BangLuongId,
                    NhanVienId = b.NhanVienId,

                    HoTenNhanVien = b.NhanVien != null
                        ? b.NhanVien.HoTen
                        : null,

                    Thang = b.Thang,
                    Nam = b.Nam,

                    LuongCoBan = b.LuongCoBan,
                    PhuCap = b.PhuCap,
                    Thuong = b.Thuong,
                    SoNgayNghiKhongLuong = b.SoNgayNghiKhongLuong,
                    TienTruNghi = b.TienTruNghi,
                    TongLuong = b.TongLuong
                })
                .FirstOrDefault();
        }

        // ADD
        public BangLuongDTO AddBangLuong(BangLuongDTO dto)
        {
            // 1. Kiểm tra nhân viên có tồn tại không
            var nhanVien = _context.NhanViens
                .FirstOrDefault(n => n.NhanVienId == dto.NhanVienId);

            if (nhanVien == null)
            {
                throw new Exception("Nhân viên không tồn tại");
            }

            var daTonTai = _context.BangLuongs
                .Any(b =>
                    b.NhanVienId == dto.NhanVienId &&
                    b.Thang == dto.Thang &&
                    b.Nam == dto.Nam);

            if (daTonTai)
            {
                throw new Exception(
                    "Nhân viên đã có bảng lương trong tháng này");
            }

            // 3. Tự động lấy lương từ bảng NhanViens
            double luongCoBan = nhanVien.LuongCoBan;
            double phuCap = nhanVien.PhuCap;
            double thuong = nhanVien.Thuong;

            // 4. Tự động tính tổng lương
            double tongLuong =
                luongCoBan +
                phuCap +
                thuong;

            // 5. Tạo bảng lương
            var bangLuong = new BangLuong
            {
                NhanVienId = dto.NhanVienId,

                Thang = dto.Thang,
                Nam = dto.Nam,

                LuongCoBan = luongCoBan,
                PhuCap = phuCap,
                Thuong = thuong,

                TongLuong = tongLuong
            };

            // 6. Lưu database
            _context.BangLuongs.Add(bangLuong);

            _context.SaveChanges();

            // 7. Trả về bảng lương vừa tạo
            return GetBangLuongById(
                bangLuong.BangLuongId)!;
        }

        // UPDATE
        public BangLuongDTO? UpdateBangLuongById(
    int id,
    BangLuongDTO dto)
        {
            // 1. Tìm bảng lương
            var bangLuong = _context.BangLuongs
                .FirstOrDefault(b => b.BangLuongId == id);

            if (bangLuong == null)
            {
                return null;
            }

            // 2. Kiểm tra nhân viên có tồn tại không
            var nhanVien = _context.NhanViens
                .FirstOrDefault(n => n.NhanVienId == dto.NhanVienId);

            if (nhanVien == null)
            {
                throw new Exception("Nhân viên không tồn tại");
            }

            // 3. Kiểm tra trùng nhân viên + tháng + năm
            var daTonTai = _context.BangLuongs
                .Any(b =>
                    b.BangLuongId != id &&
                    b.NhanVienId == dto.NhanVienId &&
                    b.Thang == dto.Thang &&
                    b.Nam == dto.Nam);

            if (daTonTai)
            {
                throw new Exception(
                    "Nhân viên đã có bảng lương trong tháng này");
            }

            // 4. Cập nhật thông tin bảng lương
            bangLuong.NhanVienId = dto.NhanVienId;

            bangLuong.Thang = dto.Thang;

            bangLuong.Nam = dto.Nam;

            // 5. LẤY LƯƠNG TỪ FORM
            bangLuong.LuongCoBan =
                dto.LuongCoBan ?? 0;

            bangLuong.PhuCap =
                dto.PhuCap ?? 0;

            bangLuong.Thuong =
                dto.Thuong ?? 0;

            // 6. TỰ ĐỘNG TÍNH TỔNG LƯƠNG
            bangLuong.TongLuong =
                (bangLuong.LuongCoBan ?? 0)
                + (bangLuong.PhuCap ?? 0)
                + (bangLuong.Thuong ?? 0);

            // 7. Lưu database
            _context.SaveChanges();

            // 8. Trả về dữ liệu sau khi cập nhật
            return GetBangLuongById(id);
        }

        // DELETE
        public bool DeleteBangLuongById(int id)
        {
            var bangLuong = _context.BangLuongs
                .FirstOrDefault(b => b.BangLuongId == id);

            if (bangLuong == null)
            {
                return false;
            }

            _context.BangLuongs.Remove(bangLuong);

            _context.SaveChanges();

            return true;
        }
        // TẠO BẢNG LƯƠNG CHO TẤT CẢ NHÂN VIÊN



        public List<BangLuongDTO> TaoBangLuongChoTatCaNhanVien(
            int thang,
            int nam)
        {
            if (thang < 1 || thang > 12)
                throw new ArgumentException("Tháng phải từ 1 đến 12.");

            if (nam < 2000 || nam > 2100)
                throw new ArgumentException("Năm không hợp lệ.");

            const double soNgayCongChuan = 26;

            var ngayDauThang = new DateTime(nam, thang, 1);
            var ngayCuoiThang = ngayDauThang.AddMonths(1).AddDays(-1);

            // Lấy ngày nghỉ lễ.
            var ngayLe = _context.Set<NgayNghiLe>()
                .Select(x => x.Ngay)
                .ToList()
                .Select(x => x.Date)
                .ToHashSet();

            // Lấy các đơn đã duyệt và không hưởng lương,
            // có khoảng thời gian giao với tháng đang tính.
            var donNghiPheps = _context.DonNghiPheps
                .Where(d =>
                    d.TrangThai == "DaDuyet" &&
                    d.HuongLuong == false &&
                    d.NgayBatDauNghi < ngayCuoiThang.AddDays(1) &&
                    d.NgayKetThucNghi >= ngayDauThang)
                .ToList();

            var nhanViens = _context.NhanViens
                .OrderBy(n => n.NhanVienId)
                .ToList();

            foreach (var nhanVien in nhanViens)
            {
                var ngayNghiKhongLuong = new HashSet<DateTime>();

                foreach (var don in donNghiPheps.Where(
                    d => d.NhanVienId == nhanVien.NhanVienId))
                {
                    var tuNgay = don.NgayBatDauNghi.Date < ngayDauThang.Date
                        ? ngayDauThang.Date
                        : don.NgayBatDauNghi.Date;

                    var denNgay = don.NgayKetThucNghi.Date > ngayCuoiThang.Date
                        ? ngayCuoiThang.Date
                        : don.NgayKetThucNghi.Date;

                    for (var ngay = tuNgay;
                         ngay <= denNgay;
                         ngay = ngay.AddDays(1))
                    {
                        if (ngay.DayOfWeek == DayOfWeek.Saturday ||
                            ngay.DayOfWeek == DayOfWeek.Sunday ||
                            ngayLe.Contains(ngay.Date))
                        {
                            continue;
                        }

                        // HashSet ngăn đếm trùng ngày nghỉ.
                        ngayNghiKhongLuong.Add(ngay.Date);
                    }
                }

                int soNgayNghiKhongLuong = ngayNghiKhongLuong.Count;

                double luongCoBan = nhanVien.LuongCoBan;
                double phuCap = nhanVien.PhuCap;
                double thuong = nhanVien.Thuong;

                double tienTruNghi =
                    luongCoBan / soNgayCongChuan * soNgayNghiKhongLuong;

                double luongSauKhiTruNghi =
                    Math.Max(0, luongCoBan - tienTruNghi);

                double tongLuong =
                    luongSauKhiTruNghi + phuCap + thuong;

                var bangLuong = _context.BangLuongs.FirstOrDefault(b =>
                    b.NhanVienId == nhanVien.NhanVienId &&
                    b.Thang == thang &&
                    b.Nam == nam);

                if (bangLuong == null)
                {
                    bangLuong = new BangLuong
                    {
                        NhanVienId = nhanVien.NhanVienId,
                        Thang = thang,
                        Nam = nam
                    };

                    _context.BangLuongs.Add(bangLuong);
                }

                bangLuong.LuongCoBan = luongCoBan;
                bangLuong.PhuCap = phuCap;
                bangLuong.Thuong = thuong;

                // Ghi đầy đủ các trường tính lương.
                bangLuong.SoNgayNghiKhongLuong = soNgayNghiKhongLuong;
                bangLuong.TienTruNghi = tienTruNghi;
                bangLuong.TongLuong = tongLuong;
            }

            _context.SaveChanges();

            return GetBangLuongTheoThangNam(thang, nam);
        }


        // LẤY BẢNG LƯƠNG THEO THÁNG VÀ NĂM
        public List<BangLuongDTO> GetBangLuongTheoThangNam(
            int thang,
            int nam)
        {
            return _context.BangLuongs
                .Where(b =>
                    b.Thang == thang &&
                    b.Nam == nam)
                .Select(b => new BangLuongDTO
                {
                    BangLuongId = b.BangLuongId,

                    NhanVienId = b.NhanVienId,

                    HoTenNhanVien =
                        b.NhanVien != null
                            ? b.NhanVien.HoTen
                            : null,

                    Thang = b.Thang,
                    Nam = b.Nam,

                    LuongCoBan = b.LuongCoBan,
                    PhuCap = b.PhuCap,
                    Thuong = b.Thuong,
                    SoNgayNghiKhongLuong = b.SoNgayNghiKhongLuong,
                    TienTruNghi = b.TienTruNghi,
                    TongLuong = b.TongLuong
                })
                .ToList();
        }
        // LẤY LƯƠNG CỦA MỘT NHÂN VIÊN
        public List<BangLuongDTO> GetBangLuongCuaNhanVien(
            int nhanVienId)
        {
            return _context.BangLuongs
                .Where(b => b.NhanVienId == nhanVienId)
                .OrderByDescending(b => b.Nam)
                .ThenByDescending(b => b.Thang)
                .Select(b => new BangLuongDTO
                {
                    BangLuongId = b.BangLuongId,

                    NhanVienId = b.NhanVienId,

                    HoTenNhanVien =
                        b.NhanVien != null
                            ? b.NhanVien.HoTen
                            : null,

                    Thang = b.Thang,

                    Nam = b.Nam,

                    LuongCoBan = b.LuongCoBan,

                    PhuCap = b.PhuCap,

                    Thuong = b.Thuong,
                    SoNgayNghiKhongLuong = b.SoNgayNghiKhongLuong,
                    TienTruNghi = b.TienTruNghi,

                    TongLuong = b.TongLuong
                })
                .ToList();
        }
        // THỐNG KÊ QUỸ LƯƠNG
        public ThongKeLuongDTO GetThongKeLuong(
            int thang,
            int nam)
        {
            var bangLuongs = _context.BangLuongs
                .Where(b =>
                    b.Thang == thang &&
                    b.Nam == nam)
                .ToList();

            return new ThongKeLuongDTO
            {
                Thang = thang,

                Nam = nam,

                SoNhanVien =
                    bangLuongs
                        .Select(b => b.NhanVienId)
                        .Distinct()
                        .Count(),

                TongLuongCoBan =
                    bangLuongs.Sum(
                        b => b.LuongCoBan ?? 0),

                TongPhuCap =
                    bangLuongs.Sum(
                        b => b.PhuCap ?? 0),

                TongThuong =
                    bangLuongs.Sum(
                        b => b.Thuong ?? 0),

                TongQuyLuong =
                    bangLuongs.Sum(
                        b => b.TongLuong ?? 0)
            };
        }
        // THỐNG KÊ QUỸ LƯƠNG THEO NĂM
        public List<ThongKeLuongNamDTO> GetThongKeLuongTheoNam(
            int nam)
        {
            var ketQua = new List<ThongKeLuongNamDTO>();

            for (int thang = 1; thang <= 12; thang++)
            {
                var bangLuongs = _context.BangLuongs
                    .Where(b =>
                        b.Thang == thang &&
                        b.Nam == nam)
                    .ToList();

                var thongKe = new ThongKeLuongNamDTO
                {
                    Nam = nam,

                    Thang = thang,

                    SoNhanVien = bangLuongs
                        .Select(b => b.NhanVienId)
                        .Distinct()
                        .Count(),

                    TongLuongCoBan = bangLuongs
                        .Sum(b => b.LuongCoBan ?? 0),

                    TongPhuCap = bangLuongs
                        .Sum(b => b.PhuCap ?? 0),

                    TongThuong = bangLuongs
                        .Sum(b => b.Thuong ?? 0),

                    TongQuyLuong = bangLuongs
                        .Sum(b => b.TongLuong ?? 0)
                };

                ketQua.Add(thongKe);
            }

            return ketQua;
        }
    }
}


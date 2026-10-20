using QuanLyNhanSu.Data;
using QuanLyNhanSu.Models.Domain;
using QuanLyNhanSu.Models.DTO;
using QuanLyNhanSu.Services;
namespace QuanLyNhanSu.Repositories
{
    public class SQLChamCongRepository : IChamCongRepository
    {
        private readonly AppDbContext _context;
        private readonly ImageStampService _imageStampService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SQLChamCongRepository(
            AppDbContext context,
            ImageStampService imageStampService,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _imageStampService = imageStampService;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET ALL
        public List<ChamCongDTO> GetAllChamCongs()
        {
            return _context.ChamCongs
                .Select(c => new ChamCongDTO
                {
                    ChamCongId = c.ChamCongId,

                    NhanVienId = c.NhanVienId,

                    HoTenNhanVien = c.NhanVien != null
                        ? c.NhanVien.HoTen
                        : null,

                    ThoiGianCham = c.ThoiGianCham,

                    HinhAnh = c.HinhAnh,

                    ViTri = c.ViTri,

                    Latitude = c.Latitude,

                    Longitude = c.Longitude
                })
                .ToList();
        }

        // GET BY ID
        public ChamCongDTO? GetChamCongById(int id)
        {
            return _context.ChamCongs
                .Where(c => c.ChamCongId == id)
                .Select(c => new ChamCongDTO
                {
                    ChamCongId = c.ChamCongId,

                    NhanVienId = c.NhanVienId,

                    HoTenNhanVien = c.NhanVien != null
                        ? c.NhanVien.HoTen
                        : null,

                    ThoiGianCham = c.ThoiGianCham,

                    HinhAnh = c.HinhAnh,

                    ViTri = c.ViTri,

                    Latitude = c.Latitude,

                    Longitude = c.Longitude
                })
                .FirstOrDefault();
        }

        // ADD CHẤM CÔNG
        public ChamCongDTO AddChamCong(ChamCongDTO dto)
        {
            var chamCong = new ChamCong
            {
                NhanVienId = dto.NhanVienId,
                ThoiGianCham = dto.ThoiGianCham,
                HinhAnh = dto.HinhAnh,
                ViTri = dto.ViTri,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude
            };

            _context.ChamCongs.Add(chamCong);
            _context.SaveChanges();

            return GetChamCongById(
                chamCong.ChamCongId)!;
        }
        // CÓ ĐÓNG DẤU ẢNH
        public async Task<ChamCongDTO> AddChamCong(
            int nhanVienId,
            AddChamCongRequestDTO request)
        {
            // KIỂM TRA ẢNH

            if (request.HinhAnh == null ||
                request.HinhAnh.Length == 0)
            {
                throw new ArgumentException(
                    "Ảnh chấm công không hợp lệ.");
            }

            // ĐỌC LATITUDE

            if (!double.TryParse(
                request.Latitude,
                System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture,
                out double latitude))
            {
                throw new ArgumentException(
                    "Latitude không hợp lệ.");
            }

            // ĐỌC LONGITUDE

            if (!double.TryParse(
                request.Longitude,
                System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture,
                out double longitude))
            {
                throw new ArgumentException(
                    "Longitude không hợp lệ.");
            }

            // ĐÓNG DẤU LÊN ẢNH

            var fileName =
                await _imageStampService.AddStampAsync(
                    request.HinhAnh,
                    request.ViTri,
                    latitude,
                    longitude);

            // TẠO URL ẢNH

            var httpRequest = _httpContextAccessor.HttpContext?.Request;

            if (httpRequest == null)
            {
                throw new InvalidOperationException(
                    "Không lấy được thông tin HTTP request.");
            }

            var imageUrl = $"{httpRequest.Scheme}://{httpRequest.Host}/Images/{Uri.EscapeDataString(fileName)}";

            // TẠO BẢN GHI CHẤM CÔNG

            var chamCong = new ChamCong
            {
                NhanVienId = nhanVienId,

                // Tự động lấy ngày giờ
                ThoiGianCham = DateTime.Now,

                // Ảnh đã được đóng dấu
                HinhAnh = imageUrl,

                // Nơi chấm công
                ViTri = request.ViTri,

                // GPS
                Latitude = latitude,

                Longitude = longitude
            };

            // LƯU DATABASE

            _context.ChamCongs.Add(chamCong);

            await _context.SaveChangesAsync();

            return GetChamCongById(
                chamCong.ChamCongId)!;
        }

        // UPDATE
        public ChamCongDTO? UpdateChamCongById(
            int id,
            ChamCongDTO dto)
        {
            var chamCong =
                _context.ChamCongs
                    .FirstOrDefault(
                        c => c.ChamCongId == id);

            if (chamCong == null)
            {
                return null;
            }

            chamCong.NhanVienId =
                dto.NhanVienId;

            chamCong.ThoiGianCham =
                dto.ThoiGianCham;

            chamCong.HinhAnh =
                dto.HinhAnh;

            chamCong.ViTri =
                dto.ViTri;

            chamCong.Latitude =
                dto.Latitude;

            chamCong.Longitude =
                dto.Longitude;

            _context.SaveChanges();

            return GetChamCongById(id);
        }

        // DELETE
        public bool DeleteChamCongById(int id)
        {
            var chamCong =
                _context.ChamCongs
                    .FirstOrDefault(
                        c => c.ChamCongId == id);

            if (chamCong == null)
            {
                return false;
            }

            _context.ChamCongs.Remove(chamCong);

            _context.SaveChanges();

            return true;
        }
    }
}

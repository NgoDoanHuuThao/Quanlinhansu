using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyNhanSu.Models.DTO;
using QuanLyNhanSu.Repositories;
using System.Security.Claims;
using System.Globalization;
namespace QuanLyNhanSu.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChamCongController : ControllerBase
    {
        private readonly IChamCongRepository _repository;
        private readonly ILogger<ChamCongController> _logger;

        public ChamCongController(
            IChamCongRepository repository,
            ILogger<ChamCongController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // GET: api/ChamCong
        [HttpGet]
        public IActionResult GetAll()
        {
            _logger.LogInformation(
                "Đang lấy danh sách chấm công");

            var chamCongs =
                _repository.GetAllChamCongs();

            // Nhân viên chỉ xem chấm công của mình
            if (User.IsInRole("NhanVien"))
            {
                var claim =
                    User.FindFirst("NhanVienId")?.Value;

                if (!int.TryParse(
                    claim,
                    out int nhanVienId))
                {
                    return Unauthorized(new
                    {
                        message =
                            "Tài khoản chưa được liên kết với nhân viên"
                    });
                }

                chamCongs = chamCongs
                    .Where(c =>
                        c.NhanVienId == nhanVienId)
                    .ToList();
            }

            return Ok(chamCongs);
        }

        // GET: api/ChamCong/1
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var chamCong =
                _repository.GetChamCongById(id);

            if (chamCong == null)
            {
                return NotFound(new
                {
                    message =
                        "Không tìm thấy dữ liệu chấm công"
                });
            }

            // Nhân viên chỉ xem chấm công của mình
            if (User.IsInRole("NhanVien"))
            {
                var claim =
                    User.FindFirst("NhanVienId")?.Value;

                if (!int.TryParse(
                    claim,
                    out int nhanVienId))
                {
                    return Unauthorized(new
                    {
                        message =
                            "Tài khoản chưa được liên kết với nhân viên"
                    });
                }

                if (chamCong.NhanVienId != nhanVienId)
                {
                    return Forbid();
                }
            }

            return Ok(chamCong);
        }

        // POST: api/ChamCong
        [HttpPost]
        public async Task<IActionResult> Add(
            [FromForm] AddChamCongRequestDTO request)
        {
            _logger.LogInformation(
                "Bắt đầu chấm công");

            // 1. LẤY NHÂN VIÊN ID TỪ JWT
            var claim =
                User.FindFirst("NhanVienId")?.Value;

            if (!int.TryParse(
                claim,
                out int nhanVienId))
            {
                return Unauthorized(new
                {
                    message =
                        "Tài khoản chưa được liên kết với nhân viên"
                });
            }

            // 2. KIỂM TRA ẢNH
            if (request.HinhAnh == null ||
                request.HinhAnh.Length == 0)
            {
                return BadRequest(new
                {
                    message =
                        "Vui lòng chọn ảnh chấm công"
                });
            }

            // 3. GIỚI HẠN DUNG LƯỢNG
            if (request.HinhAnh.Length >
                5 * 1024 * 1024)
            {
                return BadRequest(new
                {
                    message =
                        "Ảnh không được vượt quá 5MB"
                });
            }

            // 4. KIỂM TRA PHẦN MỞ RỘNG
            var extension =
                Path.GetExtension(
                    request.HinhAnh.FileName)
                .ToLowerInvariant();

            var allowedExtensions =
                new[]
                {
                    ".jpg",
                    ".jpeg",
                    ".png"
                };

            if (!allowedExtensions.Contains(
                extension))
            {
                return BadRequest(new
                {
                    message =
                        "Chỉ chấp nhận ảnh JPG, JPEG hoặc PNG"
                });
            }

            // 5. KIỂM TRA CONTENT TYPE
            var allowedContentTypes =
                new[]
                {
                    "image/jpeg",
                    "image/png"
                };

            if (!allowedContentTypes.Contains(
                request.HinhAnh.ContentType
                    .ToLowerInvariant()))
            {
                return BadRequest(new
                {
                    message =
                        "Định dạng ảnh không hợp lệ"
                });
            }

            // 6. KIỂM TRA VỊ TRÍ
            if (string.IsNullOrWhiteSpace(
                request.ViTri))
            {
                return BadRequest(new
                {
                    message =
                        "Vị trí không được để trống"
                });
            }

            if (request.ViTri.Length > 200)
            {
                return BadRequest(new
                {
                    message =
                        "Vị trí không được vượt quá 200 ký tự"
                });
            }

            // 7. LATITUDE
            if (!double.TryParse(
                request.Latitude,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out double latitude))
            {
                return BadRequest(new
                {
                    message =
                        "Latitude không hợp lệ. Ví dụ: 11.9404"
                });
            }

            // 8. LONGITUDE
            if (!double.TryParse(
                request.Longitude,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out double longitude))
            {
                return BadRequest(new
                {
                    message =
                        "Longitude không hợp lệ. Ví dụ: 108.4583"
                });
            }

            // 9. KIỂM TRA LATITUDE
            if (latitude < -90 ||
                latitude > 90)
            {
                return BadRequest(new
                {
                    message =
                        "Latitude phải từ -90 đến 90."
                });
            }

            // 10. KIỂM TRA LONGITUDE
            if (longitude < -180 ||
                longitude > 180)
            {
                return BadRequest(new
                {
                    message =
                        "Longitude phải từ -180 đến 180."
                });
            }

            // 11. KHÔNG CHO GPS = 0,0
            if (latitude == 0 &&
                longitude == 0)
            {
                return BadRequest(new
                {
                    message =
                        "Vui lòng cung cấp tọa độ GPS."
                });
            }

            // 12. TỌA ĐỘ CÔNG TY
            double latitudeCongTy = 11.9404;
            double longitudeCongTy = 108.4583;

            // Bán kính cho phép: 100 mét
            double banKinhChoPhep = 10000;

            // 13. TÍNH KHOẢNG CÁCH
            double khoangCach =
                TinhKhoangCach(
                    latitude,
                    longitude,
                    latitudeCongTy,
                    longitudeCongTy);

            // 14. GHI LOG GPS
            _logger.LogInformation(
                "GPS chấm công - Nhân viên: {NhanVienId}, Latitude: {Latitude}, Longitude: {Longitude}, Khoảng cách: {KhoangCach}m",
                nhanVienId,
                latitude,
                longitude,
                Math.Round(khoangCach, 2));

            // 15. KIỂM TRA BÁN KÍNH
            if (khoangCach > banKinhChoPhep)
            {
                return BadRequest(new
                {
                    message =
                        "Bạn đang ở ngoài khu vực chấm công",

                    khoangCach =
                        Math.Round(
                            khoangCach,
                            2),

                    banKinhChoPhep =
                        banKinhChoPhep
                });
            }

            // 16. LƯU CHẤM CÔNG + UPLOAD ẢNH
            var chamCong =
                await _repository.AddChamCong(
                    nhanVienId,
                    request);

            // 17. LOG THÀNH CÔNG
            _logger.LogInformation(
                "Chấm công thành công cho nhân viên ID: {NhanVienId}",
                nhanVienId);

            // 18. TRẢ KẾT QUẢ
            return CreatedAtAction(
                nameof(GetById),
                new
                {
                    id =
                        chamCong.ChamCongId
                },
                chamCong);
        }

        // DELETE: api/ChamCong/1
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "GiamDoc")]
        public IActionResult Delete(int id)
        {
            _logger.LogInformation(
                "Đang xóa chấm công ID: {ChamCongId}",
                id);

            var result =
                _repository.DeleteChamCongById(id);

            if (!result)
            {
                return NotFound(new
                {
                    message =
                        "Không tìm thấy dữ liệu chấm công"
                });
            }

            _logger.LogInformation(
                "Xóa chấm công ID {ChamCongId} thành công",
                id);

            return NoContent();
        }

        // TÍNH KHOẢNG CÁCH GPS
        private double TinhKhoangCach(
            double lat1,
            double lon1,
            double lat2,
            double lon2)
        {
            const double R = 6371000;

            double dLat =
                (lat2 - lat1)
                * Math.PI / 180;

            double dLon =
                (lon2 - lon1)
                * Math.PI / 180;

            double a =
                Math.Sin(dLat / 2)
                * Math.Sin(dLat / 2)
                +
                Math.Cos(
                    lat1 * Math.PI / 180)
                *
                Math.Cos(
                    lat2 * Math.PI / 180)
                *
                Math.Sin(dLon / 2)
                * Math.Sin(dLon / 2);

            double c =
                2 *
                Math.Atan2(
                    Math.Sqrt(a),
                    Math.Sqrt(1 - a));

            return R * c;
        }
    }
}
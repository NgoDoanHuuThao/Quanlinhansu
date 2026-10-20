using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyNhanSu.Models.DTO;
using QuanLyNhanSu.Repositories;

namespace QuanLyNhanSu.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BangLuongController : ControllerBase
    {
        private readonly IBangLuongRepository _repository;
        private readonly ILogger<BangLuongController> _logger;

        public BangLuongController(
            IBangLuongRepository repository,
            ILogger<BangLuongController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // GET: api/BangLuong
        [HttpGet]
        public IActionResult GetAll()
        {
            _logger.LogInformation(
                "Đang lấy danh sách bảng lương");

            var bangLuongs =
                _repository.GetAllBangLuongs();

            // Nhân viên chỉ được xem bảng lương của mình
            if (User.IsInRole("NhanVien"))
            {
                var nhanVienIdClaim =
                    User.FindFirst("NhanVienId")?.Value;

                if (!int.TryParse(
                    nhanVienIdClaim,
                    out int nhanVienId))
                {
                    return Unauthorized(new
                    {
                        message =
                            "Tài khoản chưa được liên kết với nhân viên"
                    });
                }

                bangLuongs = bangLuongs
                    .Where(b => b.NhanVienId == nhanVienId)
                    .ToList();
            }

            _logger.LogInformation(
                "Lấy bảng lương thành công. Số lượng: {Count}",
                bangLuongs.Count);

            return Ok(bangLuongs);
        }
        [HttpGet("me")]
        [Authorize(Roles = "NhanVien")]
        public IActionResult GetMySalary()
        {
            _logger.LogInformation(
                "Nhân viên đang lấy bảng lương cá nhân");

            var nhanVienIdClaim =
                User.FindFirst("NhanVienId")?.Value;

            if (!int.TryParse(
                nhanVienIdClaim,
                out int nhanVienId))
            {
                _logger.LogWarning(
                    "Tài khoản chưa có NhanVienId");

                return Unauthorized(new
                {
                    message =
                        "Tài khoản chưa được liên kết với nhân viên"
                });
            }

            var bangLuongs =
                _repository.GetBangLuongCuaNhanVien(
                    nhanVienId);

            _logger.LogInformation(
                "Lấy bảng lương cá nhân thành công. " +
                "NhanVienId: {NhanVienId}, Số lượng: {Count}",
                nhanVienId,
                bangLuongs.Count);

            return Ok(bangLuongs);
        }

        // GET: api/BangLuong/1
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            _logger.LogInformation(
                "Đang lấy bảng lương ID: {BangLuongId}",
                id);

            var bangLuong =
                _repository.GetBangLuongById(id);

            if (bangLuong == null)
            {
                _logger.LogWarning(
                    "Không tìm thấy bảng lương ID: {BangLuongId}",
                    id);

                return NotFound(new
                {
                    message = "Không tìm thấy bảng lương"
                });
            }

            // Nhân viên chỉ được xem bảng lương của mình
            if (User.IsInRole("NhanVien"))
            {
                var nhanVienIdClaim =
                    User.FindFirst("NhanVienId")?.Value;

                if (!int.TryParse(
                    nhanVienIdClaim,
                    out int nhanVienId))
                {
                    return Unauthorized(new
                    {
                        message =
                            "Tài khoản chưa được liên kết với nhân viên"
                    });
                }

                if (bangLuong.NhanVienId != nhanVienId)
                {
                    return Forbid();
                }
            }

            return Ok(bangLuong);
        }

        // POST: api/BangLuong
        [HttpPost]
        [Authorize(Roles = "GiamDoc")]
        public IActionResult Add(
    [FromBody] BangLuongDTO bangLuongDTO)
        {
            _logger.LogInformation(
                "Bắt đầu thêm bảng lương cho nhân viên ID: {NhanVienId}",
                bangLuongDTO.NhanVienId);

            if (bangLuongDTO.Thang < 1 ||
                bangLuongDTO.Thang > 12)
            {
                return BadRequest(new
                {
                    message = "Tháng phải từ 1 đến 12"
                });
            }

            if (bangLuongDTO.Nam < 2000 ||
                bangLuongDTO.Nam > 2100)
            {
                return BadRequest(new
                {
                    message = "Năm không hợp lệ"
                });
            }

            try
            {
                var bangLuong =
                    _repository.AddBangLuong(bangLuongDTO);

                _logger.LogInformation(
                    "Thêm bảng lương thành công. ID: {BangLuongId}",
                    bangLuong.BangLuongId);

                return CreatedAtAction(
                    nameof(GetById),
                    new
                    {
                        id = bangLuong.BangLuongId
                    },
                    bangLuong);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Không thể thêm bảng lương cho nhân viên ID: {NhanVienId}",
                    bangLuongDTO.NhanVienId);

                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // PUT: api/BangLuong/1
        [HttpPut("{id:int}")]
        [Authorize(Roles = "GiamDoc")]
        public IActionResult Update(
            int id,
            [FromBody] BangLuongDTO bangLuongDTO)
        {
            _logger.LogInformation(
                "Đang cập nhật bảng lương ID: {BangLuongId}",
                id);

            // Kiểm tra tháng
            if (bangLuongDTO.Thang < 1 ||
                bangLuongDTO.Thang > 12)
            {
                return BadRequest(new
                {
                    message = "Tháng phải từ 1 đến 12"
                });
            }

            // Kiểm tra năm
            if (bangLuongDTO.Nam < 2000 ||
                bangLuongDTO.Nam > 2100)
            {
                return BadRequest(new
                {
                    message = "Năm không hợp lệ"
                });
            }

            var bangLuong =
                _repository.UpdateBangLuongById(
                    id,
                    bangLuongDTO);

            if (bangLuong == null)
            {
                _logger.LogWarning(
                    "Không tìm thấy bảng lương ID: {BangLuongId}",
                    id);

                return NotFound(new
                {
                    message = "Không tìm thấy bảng lương"
                });
            }

            _logger.LogInformation(
                "Cập nhật bảng lương ID {BangLuongId} thành công",
                id);

            return Ok(bangLuong);
        }

        // DELETE: api/BangLuong/1
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "GiamDoc")]
        public IActionResult Delete(int id)
        {
            _logger.LogInformation(
                "Đang xóa bảng lương ID: {BangLuongId}",
                id);

            var result =
                _repository.DeleteBangLuongById(id);

            if (!result)
            {
                _logger.LogWarning(
                    "Không tìm thấy bảng lương ID: {BangLuongId}",
                    id);

                return NotFound(new
                {
                    message = "Không tìm thấy bảng lương"
                });
            }

            _logger.LogInformation(
                "Xóa bảng lương ID {BangLuongId} thành công",
                id);

            return NoContent();
        }
        [HttpPost("tao-tat-ca")]
        [Authorize(Roles = "GiamDoc")]
        public IActionResult TaoBangLuongChoTatCaNhanVien(
    [FromBody] TaoBangLuongRequestDTO request)
        {
            _logger.LogInformation(
                "Bắt đầu tạo bảng lương cho tất cả nhân viên. Tháng: {Thang}, Năm: {Nam}",
                request.Thang,
                request.Nam);

            try
            {
                var ketQua =
                    _repository.TaoBangLuongChoTatCaNhanVien(
                        request.Thang,
                        request.Nam);

                _logger.LogInformation(
                    "Tạo bảng lương hoàn tất. Tháng: {Thang}, Năm: {Nam}, Số lượng: {Count}",
                    request.Thang,
                    request.Nam,
                    ketQua.Count);

                return Ok(new
                {
                    message =
                        "Tạo bảng lương cho tất cả nhân viên thành công",

                    thang = request.Thang,

                    nam = request.Nam,

                    soLuong =
                        ketQua.Count,

                    danhSach =
                        ketQua
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Lỗi khi tạo bảng lương cho tất cả nhân viên");

                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
        // GET: api/BangLuong/theo-thang-nam?thang=9&nam=2026
        [HttpGet("theo-thang-nam")]
        public IActionResult GetTheoThangNam(
            [FromQuery] int thang,
            [FromQuery] int nam)
        {
            _logger.LogInformation(
                "Lấy bảng lương tháng {Thang}/{Nam}",
                thang,
                nam);

            if (thang < 1 || thang > 12)
            {
                return BadRequest(new
                {
                    message = "Tháng phải từ 1 đến 12"
                });
            }

            if (nam < 2000 || nam > 2100)
            {
                return BadRequest(new
                {
                    message = "Năm không hợp lệ"
                });
            }

            var bangLuongs =
                _repository.GetBangLuongTheoThangNam(
                    thang,
                    nam);

            // Nhân viên chỉ được xem bảng lương của mình
            if (User.IsInRole("NhanVien"))
            {
                var nhanVienIdClaim =
                    User.FindFirst("NhanVienId")?.Value;

                if (!int.TryParse(
                    nhanVienIdClaim,
                    out int nhanVienId))
                {
                    return Unauthorized(new
                    {
                        message =
                            "Tài khoản chưa được liên kết với nhân viên"
                    });
                }

                bangLuongs = bangLuongs
                    .Where(b =>
                        b.NhanVienId == nhanVienId)
                    .ToList();
            }

            return Ok(bangLuongs);
        }
        // GET: api/BangLuong/thong-ke?thang=9&nam=2026
        [HttpGet("thong-ke")]
        [Authorize(Roles = "GiamDoc")]
        public IActionResult GetThongKe(
            [FromQuery] int thang,
            [FromQuery] int nam)
        {
            _logger.LogInformation(
                "Thống kê quỹ lương tháng {Thang}/{Nam}",
                thang,
                nam);

            if (thang < 1 || thang > 12)
            {
                return BadRequest(new
                {
                    message = "Tháng phải từ 1 đến 12"
                });
            }

            if (nam < 2000 || nam > 2100)
            {
                return BadRequest(new
                {
                    message = "Năm không hợp lệ"
                });
            }

            var thongKe =
                _repository.GetThongKeLuong(
                    thang,
                    nam);

            return Ok(thongKe);
        }
        // GET: api/BangLuong/thong-ke-nam?nam=2026
        [HttpGet("thong-ke-nam")]
        [Authorize(Roles = "GiamDoc")]
        public IActionResult GetThongKeTheoNam(
            [FromQuery] int nam)
        {
            _logger.LogInformation(
                "Thống kê quỹ lương cả năm {Nam}",
                nam);

            if (nam < 2000 || nam > 2100)
            {
                return BadRequest(new
                {
                    message = "Năm không hợp lệ"
                });
            }

            var ketQua =
                _repository.GetThongKeLuongTheoNam(nam);

            return Ok(ketQua);
        }
    }
}

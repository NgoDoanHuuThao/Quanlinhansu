using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhanSu.Data;
using QuanLyNhanSu.Models.Domain;
using QuanLyNhanSu.Models.DTO;
using QuanLyNhanSu.Repositories;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace QuanLyNhanSu.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NhanVienController : ControllerBase
    {
        private readonly INhanVienRepository _nhanVienRepository;
        private readonly ILogger<NhanVienController> _logger;

        public NhanVienController(
            INhanVienRepository nhanVienRepository,
            ILogger<NhanVienController> logger)
        {
            _nhanVienRepository = nhanVienRepository;
            _logger = logger;
        }

        // GET: api/NhanVien
        [HttpGet]
        public IActionResult GetAll(
            [FromQuery] string? filterOn,
            [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy,
            [FromQuery] bool isAscending = true,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value
                ?? User.FindFirst("role")?.Value;

            var claim = User.FindFirst("NhanVienId")?.Value;

            // Nhân viên chỉ xem thông tin của mình
            if (role == "NhanVien")
            {
                if (!int.TryParse(claim, out int nhanVienId))
                {
                    return Unauthorized(new
                    {
                        message = "Không tìm thấy NhanVienId trong token."
                    });
                }

                var nhanVien =
                    _nhanVienRepository.GetNhanVienById(nhanVienId);

                if (nhanVien == null)
                    return NotFound();

                return Ok(new List<NhanVienDTO> { nhanVien });
            }

            // Giám đốc xem toàn bộ danh sách
            if (role == "GiamDoc")
            {
                var danhSach = _nhanVienRepository.GetAllNhanViens(
                    filterOn, filterQuery, sortBy,
                    isAscending, pageNumber, pageSize);

                return Ok(danhSach);
            }

            // Trưởng phòng xem nhân viên cùng phòng ban
            if (role == "TruongPhong")
            {
                if (!int.TryParse(claim, out int truongPhongId))
                {
                    return Unauthorized(new
                    {
                        message = "Không tìm thấy NhanVienId trong token."
                    });
                }

                var truongPhong =
                    _nhanVienRepository.GetNhanVienById(truongPhongId);

                if (truongPhong == null)
                {
                    return NotFound(new
                    {
                        message = "Không tìm thấy thông tin Trưởng phòng."
                    });
                }

                int phongBanId = truongPhong.PhongBanId;

                var danhSach = _nhanVienRepository.GetAllNhanViens(
                    filterOn, filterQuery, sortBy,
                    isAscending, pageNumber, pageSize);

                var danhSachCungPhong = danhSach
                    .Where(nv => nv.PhongBanId == phongBanId)
                    .ToList();

                return Ok(danhSachCungPhong);
            }

            return Forbid();
        } // Kết thúc GetAll()

        // GET: api/NhanVien/me
        [HttpGet("me")]
        [Authorize(Roles = "NhanVien")]
        public IActionResult GetMyProfile()
        {
            var claim = User.FindFirst("NhanVienId");

            if (claim == null ||
                !int.TryParse(claim.Value, out int nhanVienId))
            {
                return Unauthorized(new
                {
                    message = "NhanVienId không hợp lệ hoặc không tồn tại."
                });
            }

            var nhanVien =
                _nhanVienRepository.GetNhanVienById(nhanVienId);

            if (nhanVien == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy thông tin nhân viên."
                });
            }

            return Ok(nhanVien);
        }

        // GET: api/NhanVien/1
        [HttpGet("{id:int}")]
        [Authorize(Roles = "GiamDoc")]
        public IActionResult GetById(int id)
        {
            var nhanVien =
                _nhanVienRepository.GetNhanVienById(id);

            if (nhanVien == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy nhân viên."
                });
            }

            return Ok(nhanVien);
        }

        // POST: api/NhanVien
        [HttpPost]
        [Authorize(Roles = "GiamDoc")]
        public IActionResult Add(
            [FromBody] AddNhanVienRequestDTO request)
        {
            var nhanVien =
                _nhanVienRepository.AddNhanVien(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = nhanVien.NhanVienId },
                nhanVien);
        }

        // PUT: api/NhanVien/1
        [HttpPut("{id:int}")]
        [Authorize(Roles = "GiamDoc")]
        public IActionResult Update(
            int id,
            [FromBody] AddNhanVienRequestDTO request)
        {
            var nhanVien =
                _nhanVienRepository.UpdateNhanVienById(id, request);

            if (nhanVien == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy nhân viên."
                });
            }

            return Ok(nhanVien);
        }

        // DELETE: api/NhanVien/1
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "GiamDoc")]
        public IActionResult Delete(int id)
        {
            var result =
                _nhanVienRepository.DeleteNhanVienById(id);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy nhân viên."
                });
            }

            return Ok(new
            {
                message = "Xóa nhân viên thành công."
            });
        }
    }
}
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using QuanLyNhanSu.Models.DTO;
using QuanLyNhanSu.Repositories;

namespace QuanLyNhanSu.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ThongBaoController : ControllerBase
    {
        private readonly IThongBaoRepository repository;

        public ThongBaoController(IThongBaoRepository repository)
        {
            this.repository = repository;
        }

        private int? LayNhanVienId()
        {
            var claim =
                User.FindFirst("NhanVienId") ??
                User.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null ||
                !int.TryParse(claim.Value, out int id))
                return null;

            return id;
        }

        private bool CoVaiTro(params string[] roles)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value
                       ?? User.FindFirst("role")?.Value;

            return role != null &&
                   roles.Contains(role, StringComparer.OrdinalIgnoreCase);
        }

        // GET: api/ThongBao
        // Lấy thông báo của nhân viên đang đăng nhập
        [HttpGet]
        public IActionResult GetThongBaoCuaToi()
        {
            var nhanVienId = LayNhanVienId();

            if (nhanVienId == null)
                return Unauthorized(new
                {
                    message = "Token chưa có NhanVienId hợp lệ."
                });

            var danhSach =
                repository.GetThongBaoCuaNhanVien(nhanVienId.Value);

            return Ok(danhSach);
        }

        // GET: api/ThongBao/quan-ly
        [HttpGet("quan-ly")]
        [Authorize(Roles = "GiamDoc,TruongPhong")]
        public IActionResult GetAllThongBao()
        {
            return Ok(repository.GetAllThongBao());
        }

        // GET: api/ThongBao/5
        [HttpGet("{id:int}")]
        public IActionResult GetThongBaoById(int id)
        {
            var thongBao = repository.GetThongBaoById(id);

            if (thongBao == null)
                return NotFound(new
                {
                    message = "Không tìm thấy thông báo."
                });

            return Ok(thongBao);
        }

        // POST: api/ThongBao
        // Gửi thông báo
        [HttpPost]
        [Authorize(Roles = "GiamDoc,TruongPhong")]
        public IActionResult AddThongBao(
            [FromBody] AddThongBaoRequestDTO request)
        {
            var nguoiGuiId = LayNhanVienId();

            if (nguoiGuiId == null)
                return Unauthorized(new
                {
                    message = "Token chưa có NhanVienId hợp lệ."
                });

            try
            {
                var thongBao =
                    repository.AddThongBao(request, nguoiGuiId.Value);

                return CreatedAtAction(
                    nameof(GetThongBaoById),
                    new { id = thongBao.ThongBaoId },
                    thongBao);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/ThongBao/5/da-doc
        [HttpPut("{id:int}/da-doc")]
        public IActionResult DanhDauDaDoc(int id)
        {
            var nhanVienId = LayNhanVienId();

            if (nhanVienId == null)
                return Unauthorized();

            var result =
                repository.CapNhatDaDoc(id, nhanVienId.Value);

            if (!result)
                return NotFound(new
                {
                    message = "Không tìm thấy thông báo dành cho bạn."
                });

            return Ok(new
            {
                message = "Đã đánh dấu thông báo đã đọc."
            });
        }

        // DELETE: api/ThongBao/5
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "GiamDoc")]
        public IActionResult DeleteThongBao(int id)
        {
            var result = repository.DeleteThongBao(id);

            if (!result)
                return NotFound(new
                {
                    message = "Không tìm thấy thông báo."
                });

            return Ok(new
            {
                message = "Xóa thông báo thành công."
            });
        }
    }
}

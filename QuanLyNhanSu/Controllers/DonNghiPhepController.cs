using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using QuanLyNhanSu.Models.Domain;
using QuanLyNhanSu.Models.DTO;
using QuanLyNhanSu.Repositories;

namespace QuanLyNhanSu.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DonNghiPhepController : ControllerBase
    {
        private readonly IDonNghiPhepRepository _repository;

        public DonNghiPhepController(
            IDonNghiPhepRepository repository)
        {
            _repository = repository;
        }

        private int? LayNhanVienId()
        {
            var value = User.FindFirst("NhanVienId")?.Value;
            return int.TryParse(value, out int id) ? id : null;
        }

        private string LayVaiTro()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value
                ?? User.FindFirst("role")?.Value
                ?? string.Empty;
        }

        // Chuyển entity sang DTO để trả đầy đủ thông tin cho MVC
        private static DonNghiPhepDTO ChuyenSangDTO(DonNghiPhep don)
        {
            return new DonNghiPhepDTO
            {
                DonNghiPhepId = don.DonNghiPhepId,
                NhanVienId = don.NhanVienId,

                HoTenNhanVien = don.NhanVien?.HoTen,
                TenPhongBan = don.NhanVien?.PhongBan?.TenPhongBan,

                NgayVietDon = don.NgayVietDon,
                NgayBatDauNghi = don.NgayBatDauNghi,
                NgayKetThucNghi = don.NgayKetThucNghi,
                SoNgayNghi = don.SoNgayNghi,
                LyDoNghi = don.LyDoNghi,

                TrangThai = don.TrangThai,
                HuongLuong = don.HuongLuong,
                NguoiDuyetId = don.NguoiDuyetId,
                NgayDuyet = don.NgayDuyet,
                GhiChuDuyet = don.GhiChuDuyet
            };
        }

        // GET: api/DonNghiPhep
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var vaiTro = LayVaiTro();
            var nhanVienId = LayNhanVienId();

            if (vaiTro == "GiamDoc")
            {
                var tatCa = await _repository.GetAllAsync();

                return Ok(tatCa.Select(ChuyenSangDTO).ToList());
            }

            if (nhanVienId == null)
            {
                return Unauthorized(
                    "Không xác định được nhân viên đăng nhập.");
            }

            var danhSach = await _repository.GetByNhanVienIdAsync(
                nhanVienId.Value);

            return Ok(danhSach.Select(ChuyenSangDTO).ToList());
        }

        // GET: api/DonNghiPhep/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var don = await _repository.GetByIdAsync(id);

            if (don == null)
            {
                return NotFound("Không tìm thấy đơn nghỉ phép.");
            }

            if (LayVaiTro() != "GiamDoc")
            {
                var nhanVienId = LayNhanVienId();

                if (nhanVienId == null)
                {
                    return Unauthorized();
                }

                if (don.NhanVienId != nhanVienId.Value)
                {
                    return Forbid();
                }
            }

            return Ok(ChuyenSangDTO(don));
        }

        // POST: api/DonNghiPhep
        [HttpPost]
        [Authorize(Roles = "NhanVien,TruongPhong")]
        public async Task<IActionResult> TaoDon(
            [FromBody] TaoDonNghiPhepRequestDTO request)
        {
            var nhanVienId = LayNhanVienId();

            if (nhanVienId == null)
            {
                return Unauthorized(
                    "Không xác định được nhân viên đăng nhập.");
            }

            if (string.IsNullOrWhiteSpace(request.LyDoNghi))
            {
                return BadRequest("Vui lòng nhập lý do nghỉ.");
            }

            var ngayBatDau = request.NgayBatDauNghi.Date;
            var ngayKetThuc = request.NgayKetThucNghi.Date;

            if (ngayBatDau < DateTime.Today)
            {
                return BadRequest(
                    "Ngày bắt đầu nghỉ không được ở quá khứ.");
            }

            if (ngayKetThuc < ngayBatDau)
            {
                return BadRequest(
                    "Ngày kết thúc phải từ ngày bắt đầu trở đi.");
            }

            var don = new DonNghiPhep
            {
                NhanVienId = nhanVienId.Value,
                NgayVietDon = DateTime.Today,
                NgayBatDauNghi = ngayBatDau,
                NgayKetThucNghi = ngayKetThuc,
                SoNgayNghi = (ngayKetThuc - ngayBatDau).Days + 1,
                LyDoNghi = request.LyDoNghi.Trim(),
                TrangThai = "ChoDuyet"
            };

            var ketQua = await _repository.CreateAsync(don);

            // Đọc lại đơn để lấy cả tên nhân viên và phòng ban
            var donDayDu = await _repository.GetByIdAsync(
                ketQua.DonNghiPhepId);

            if (donDayDu == null)
            {
                return StatusCode(500,
                    "Tạo đơn thành công nhưng không tải lại được dữ liệu.");
            }

            return CreatedAtAction(
                nameof(GetById),
                new { id = donDayDu.DonNghiPhepId },
                ChuyenSangDTO(donDayDu));
        }

        // PUT: api/DonNghiPhep/5/duyet
        [HttpPut("{id:int}/duyet")]
        [Authorize(Roles = "TruongPhong,GiamDoc")]
        public async Task<IActionResult> DuyetDon(
            int id,
            [FromBody] DuyetDonNghiPhepRequestDTO request)
        {
            var nguoiDuyetId = LayNhanVienId();

            if (nguoiDuyetId == null)
            {
                return Unauthorized(
                    "Không xác định được người duyệt.");
            }

            var trangThai = request.TrangThai?.Trim();

            if (trangThai != "DaDuyet" &&
                trangThai != "TuChoi")
            {
                return BadRequest(
                    "Trạng thái chỉ được là DaDuyet hoặc TuChoi.");
            }

            if (trangThai == "TuChoi" &&
                string.IsNullOrWhiteSpace(request.GhiChuDuyet))
            {
                return BadRequest(
                    "Vui lòng nhập lý do từ chối đơn nghỉ phép.");
            }

            var don = await _repository.GetByIdAsync(id);

            if (don == null)
            {
                return NotFound("Không tìm thấy đơn nghỉ phép.");
            }

            if (don.TrangThai != "ChoDuyet")
            {
                return BadRequest("Đơn này đã được xử lý.");
            }

            don.TrangThai = trangThai!;

            // Chỉ áp dụng loại nghỉ khi đơn được duyệt
            if (trangThai == "DaDuyet")
            {
                don.HuongLuong = request.HuongLuong;
            }

            don.NguoiDuyetId = nguoiDuyetId.Value;
            don.NgayDuyet = DateTime.Now;
            don.GhiChuDuyet = string.IsNullOrWhiteSpace(
                request.GhiChuDuyet)
                    ? null
                    : request.GhiChuDuyet.Trim();

            var capNhatThanhCong =
                await _repository.UpdateAsync(don);

            if (!capNhatThanhCong)
            {
                return StatusCode(500,
                    "Không lưu được kết quả xử lý đơn nghỉ phép.");
            }

            // Đọc lại để trả về dữ liệu đã lưu
            var donDaCapNhat = await _repository.GetByIdAsync(id);

            if (donDaCapNhat == null)
            {
                return NotFound("Không tìm thấy đơn sau khi cập nhật.");
            }

            return Ok(ChuyenSangDTO(donDaCapNhat));
        }
    }
}

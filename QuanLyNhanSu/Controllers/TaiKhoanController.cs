using Microsoft.AspNetCore.Mvc;
using QuanLyNhanSu.Models.DTO;
using QuanLyNhanSu.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace QuanLyNhanSu.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaiKhoanController : ControllerBase
    {
        private readonly ITaiKhoanRepository _repository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TaiKhoanController> _logger;

        public TaiKhoanController(
            ITaiKhoanRepository repository,
            IConfiguration configuration,
            ILogger<TaiKhoanController> logger)
        {
            _repository = repository;
            _configuration = configuration;
            _logger = logger;
        }


        // GET ALL
        // CHỈ GIÁM ĐỐC

        [HttpGet]
        [Authorize(Roles = "GiamDoc")]
        public IActionResult GetAll()
        {
            var taiKhoans =
                _repository.GetAllTaiKhoans();

            return Ok(taiKhoans);
        }


        // GET BY ID
        // CHỈ GIÁM ĐỐC

        [HttpGet("{id:int}")]
        [Authorize(Roles = "GiamDoc")]
        public IActionResult GetById(int id)
        {
            var taiKhoan =
                _repository.GetTaiKhoanById(id);

            if (taiKhoan == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy tài khoản"
                });
            }

            return Ok(taiKhoan);
        }


        // ADD

        [HttpPost]
        [Authorize(Roles = "GiamDoc")]
        public IActionResult Add(
            [FromBody] AddTaiKhoanRequestDTO request)
        {
            // KIỂM TRA DỮ LIỆU

            if (request == null)
            {
                return BadRequest(new
                {
                    message = "Dữ liệu không hợp lệ."
                });
            }


            if (string.IsNullOrWhiteSpace(
                request.TenDangNhap))
            {
                return BadRequest(new
                {
                    message =
                        "Tên đăng nhập không được để trống."
                });
            }


            if (string.IsNullOrWhiteSpace(
                request.MatKhau))
            {
                return BadRequest(new
                {
                    message =
                        "Mật khẩu không được để trống."
                });
            }


            if (request.MatKhau.Length < 6)
            {
                return BadRequest(new
                {
                    message =
                        "Mật khẩu phải có ít nhất 6 ký tự."
                });
            }


            if (request.NhanVienId <= 0)
            {
                return BadRequest(new
                {
                    message =
                        "Nhân viên không hợp lệ."
                });
            }


            // KIỂM TRA VAI TRÒ

            var vaiTroHopLe =
                new[]
                {
                    "Giám đốc",
                    "Trưởng phòng",
                    "Nhân viên"
                };

            if (!vaiTroHopLe.Contains(
                request.VaiTro))
            {
                return BadRequest(new
                {
                    message =
                        "Vai trò không hợp lệ."
                });
            }


            // KIỂM TRA TRÙNG TÊN ĐĂNG NHẬP

            if (_repository.IsTenDangNhapTonTai(
                request.TenDangNhap))
            {
                return BadRequest(new
                {
                    message =
                        "Tên đăng nhập đã tồn tại."
                });
            }


            // KIỂM TRA NHÂN VIÊN ĐÃ CÓ TÀI KHOẢN

            if (_repository.IsNhanVienDaCoTaiKhoan(
                request.NhanVienId))
            {
                return BadRequest(new
                {
                    message =
                        "Nhân viên này đã có tài khoản."
                });
            }

            // THÊM TÀI KHOẢN

            var taiKhoan =
                _repository.AddTaiKhoan(request);


            return CreatedAtAction(
                nameof(GetById),
                new
                {
                    id = taiKhoan.TaiKhoanId
                },
                taiKhoan);
        }


        // UPDATE
        // CHỈ GIÁM ĐỐC

        [HttpPut("{id:int}")]
        [Authorize(Roles = "GiamDoc")]
        public IActionResult Update(
            int id,
            [FromBody] TaiKhoanDTO taiKhoanDTO)
        {
            var taiKhoan =
                _repository.UpdateTaiKhoanById(
                    id,
                    taiKhoanDTO);

            if (taiKhoan == null)
            {
                return NotFound(new
                {
                    message =
                        "Không tìm thấy tài khoản"
                });
            }

            return Ok(taiKhoan);
        }


        // DELETE
        // CHỈ GIÁM ĐỐC

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "GiamDoc")]
        public IActionResult Delete(int id)
        {
            var result =
                _repository.DeleteTaiKhoanById(id);

            if (!result)
            {
                return NotFound(new
                {
                    message =
                        "Không tìm thấy tài khoản"
                });
            }

            return Ok(new
            {
                message =
                    "Xóa tài khoản thành công"
            });
        }


        // LOGIN
        // KHÔNG CẦN JWT

        [HttpPost("login")]
        public IActionResult Login(
            [FromBody] LoginRequestDTO request)
        {
            _logger.LogInformation(
                "Đang xử lý đăng nhập cho tài khoản: {TenDangNhap}",
                request.TenDangNhap);


            var taiKhoan =
                _repository.Login(request);


            if (taiKhoan == null)
            {
                _logger.LogWarning(
                    "Đăng nhập thất bại cho tài khoản: {TenDangNhap}",
                    request.TenDangNhap);

                return Unauthorized(new
                {
                    message =
                        "Tên đăng nhập hoặc mật khẩu không đúng"
                });
            }


            // JWT CONFIG

            var jwtKey =
                _configuration["Jwt:Key"];

            var jwtIssuer =
                _configuration["Jwt:Issuer"];

            var jwtAudience =
                _configuration["Jwt:Audience"];


            if (string.IsNullOrEmpty(jwtKey))
            {
                _logger.LogError(
                    "JWT Key chưa được cấu hình trong appsettings.json");

                return StatusCode(500, new
                {
                    message =
                        "Chưa cấu hình Jwt:Key"
                });
            }


            // CHUẨN HÓA ROLE

            var role =
                taiKhoan.VaiTro;


            if (role == "Giám đốc")
            {
                role = "GiamDoc";
            }
            else if (role == "Trưởng phòng")
            {
                role = "TruongPhong";
            }
            else if (role == "Nhân viên")
            {
                role = "NhanVien";
            }


            // TẠO CLAIM
            var claims =
                new List<Claim>
                {
                    new Claim(
                        JwtRegisteredClaimNames.Sub,
                        taiKhoan.TaiKhoanId.ToString()),

                    new Claim(
                        JwtRegisteredClaimNames.UniqueName,
                        taiKhoan.TenDangNhap),

                    new Claim(
                        ClaimTypes.Role,
                        role),

                    new Claim(
                        "TaiKhoanId",
                        taiKhoan.TaiKhoanId.ToString())
                };


            // NHANVIEN ID

            if (taiKhoan.NhanVienId.HasValue)
            {
                claims.Add(
                    new Claim(
                        "NhanVienId",
                        taiKhoan.NhanVienId.Value.ToString()));
            }

            // TẠO JWT

            var key =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey));


            var credentials =
                new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256);


            var expireMinutes =
                _configuration.GetValue<int?>(
                    "Jwt:ExpireMinutes")
                ?? 60;


            var token =
                new JwtSecurityToken(
                    issuer: jwtIssuer,
                    audience: jwtAudience,
                    claims: claims,
                    expires:
                        DateTime.UtcNow.AddMinutes(
                            expireMinutes),
                    signingCredentials:
                        credentials
                );


            var tokenString =
                new JwtSecurityTokenHandler()
                    .WriteToken(token);


            _logger.LogInformation(
                "Đăng nhập thành công. TaiKhoanId: {TaiKhoanId}, Role: {Role}",
                taiKhoan.TaiKhoanId,
                role);


            return Ok(new
            {
                message =
                    "Đăng nhập thành công",

                token =
                    tokenString,

                expiresInMinutes =
                    expireMinutes,

                taiKhoan = new
                {
                    taiKhoan.TaiKhoanId,

                    taiKhoan.TenDangNhap,

                    VaiTro = role,

                    taiKhoan.NhanVienId
                }
            });
        }
    }
}

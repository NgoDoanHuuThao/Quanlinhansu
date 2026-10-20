using Microsoft.AspNetCore.Mvc;
using QuanLyNhanSuMVC.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace QuanLyNhanSuMVC.Controllers
{
    public class LichSuChamCongController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LichSuChamCongController(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // LẤY HTTP CLIENT + JWT

        private HttpClient GetClient()
        {
            var client = _httpClientFactory
                .CreateClient("QuanLyNhanSuAPI");

            var token = HttpContext.Session
                .GetString("JWT");

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(
                        "Bearer",
                        token);
            }

            return client;
        }

        private static string? NormalizeImageUrl(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return null;

            imageUrl = imageUrl.Trim();

            const string apiBaseUrl = "https://localhost:7151";
            const string duplicatePrefix = apiBaseUrl + "/" + apiBaseUrl + "/";

            // Loại bỏ địa chỉ API bị lặp
            while (imageUrl.StartsWith(
                duplicatePrefix,
                StringComparison.OrdinalIgnoreCase))
            {
                imageUrl = apiBaseUrl + "/" +
                    imageUrl.Substring(duplicatePrefix.Length);
            }

            // Nếu đã là URL đầy đủ thì giữ nguyên
            if (Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttp ||
                    uri.Scheme == Uri.UriSchemeHttps))
            {
                return uri.ToString();
            }

            // Nếu là đường dẫn tương đối
            imageUrl = imageUrl.TrimStart('/');

            return $"{apiBaseUrl}/{imageUrl}";
        }


        // KIỂM TRA ĐĂNG NHẬP

        private bool IsLoggedIn()
        {
            return !string.IsNullOrEmpty(
                HttpContext.Session.GetString("JWT"));
        }


        // LẤY ROLE

        private string GetRole()
        {
            return HttpContext.Session
                .GetString("Role") ?? "";
        }


        // LẤY NHÂN VIÊN ID ĐANG ĐĂNG NHẬP

        private int GetNhanVienId()
        {
            var id = HttpContext.Session
                .GetString("NhanVienId");

            if (int.TryParse(
                id,
                out int nhanVienId))
            {
                return nhanVienId;
            }

            return 0;
        }


        // TRANG CHÍNH LỊCH SỬ CHẤM CÔNG

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            var role = GetRole();

            try
            {
                var client = GetClient();


                // NHÂN VIÊN

                if (role == "NhanVien")
                {
                    return RedirectToAction(
                        "CuaToi");
                }


                // GIÁM ĐỐC / TRƯỞNG PHÒNG

                var responseNhanVien =
                    await client.GetAsync(
                        "api/NhanVien");


                if (!responseNhanVien.IsSuccessStatusCode)
                {
                    ViewBag.Error =
                        "Không thể lấy danh sách nhân viên.";

                    return View(
                        "DanhSachNhanVien",
                        new List<NhanVienModel>());
                }


                var jsonNhanVien =
                    await responseNhanVien.Content
                        .ReadAsStringAsync();


                var nhanViens =
                    JsonSerializer.Deserialize<
                        List<NhanVienModel>>(
                            jsonNhanVien,
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            })
                    ?? new List<NhanVienModel>();


                // TRƯỞNG PHÒNG
                // CHỈ XEM NHÂN VIÊN CÙNG PHÒNG

                if (role == "TruongPhong")
                {
                    var currentNhanVienId =
                        GetNhanVienId();


                    if (currentNhanVienId == 0)
                    {
                        ViewBag.Error =
                            "Không xác định được nhân viên của tài khoản.";

                        return View(
                            "DanhSachNhanVien",
                            new List<NhanVienModel>());
                    }


                    var currentNhanVien =
                        nhanViens.FirstOrDefault(
                            x =>
                                x.NhanVienId ==
                                currentNhanVienId);


                    if (currentNhanVien == null)
                    {
                        ViewBag.Error =
                            "Không tìm thấy thông tin nhân viên.";

                        return View(
                            "DanhSachNhanVien",
                            new List<NhanVienModel>());
                    }


                    nhanViens =
                        nhanViens
                            .Where(
                                x =>
                                    x.PhongBanId ==
                                    currentNhanVien.PhongBanId)
                            .ToList();
                }


                ViewBag.Role = role;


                return View(
                    "DanhSachNhanVien",
                    nhanViens);
            }
            catch (Exception ex)
            {
                ViewBag.Error =
                    "Lỗi: " + ex.Message;

                return View(
                    "DanhSachNhanVien",
                    new List<NhanVienModel>());
            }
        }


        // XEM LỊCH SỬ CHẤM CÔNG CỦA MỘT NHÂN VIÊN

        [HttpGet]
        public async Task<IActionResult> NhanVien(int id)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            var role = GetRole();

            try
            {
                var client = GetClient();


                // NHÂN VIÊN
                // CHỈ ĐƯỢC XEM CHÍNH MÌNH

                if (role == "NhanVien")
                {
                    var currentId =
                        GetNhanVienId();


                    if (currentId == 0 ||
                        id != currentId)
                    {
                        return Forbid();
                    }
                }


                // LẤY DANH SÁCH NHÂN VIÊN

                var responseNhanVien =
                    await client.GetAsync(
                        "api/NhanVien");


                if (!responseNhanVien.IsSuccessStatusCode)
                {
                    return Content(
                        "Không thể lấy thông tin nhân viên.");
                }


                var jsonNhanVien =
                    await responseNhanVien.Content
                        .ReadAsStringAsync();


                var nhanViens =
                    JsonSerializer.Deserialize<
                        List<NhanVienModel>>(
                            jsonNhanVien,
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            })
                    ?? new List<NhanVienModel>();


                var nhanVien =
                    nhanViens.FirstOrDefault(
                        x =>
                            x.NhanVienId == id);


                if (nhanVien == null)
                {
                    return NotFound(
                        "Không tìm thấy nhân viên.");
                }


                // TRƯỞNG PHÒNG
                // CHỈ XEM NHÂN VIÊN CÙNG PHÒNG

                if (role == "TruongPhong")
                {
                    var currentId =
                        GetNhanVienId();


                    if (currentId == 0)
                    {
                        return Forbid();
                    }


                    var currentNhanVien =
                        nhanViens.FirstOrDefault(
                            x =>
                                x.NhanVienId ==
                                currentId);


                    if (currentNhanVien == null)
                    {
                        return Forbid();
                    }


                    if (nhanVien.PhongBanId !=
                        currentNhanVien.PhongBanId)
                    {
                        return Forbid();
                    }
                }


                // LẤY LỊCH SỬ CHẤM CÔNG

                var response =
                    await client.GetAsync(
                        "api/ChamCong");


                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error =
                        "Không thể lấy lịch sử chấm công.";

                    return View(
                        "ChiTiet",
                        new List<LichSuChamCongModel>());
                }


                var json =
                    await response.Content
                        .ReadAsStringAsync();


                var chamCongs =
                    JsonSerializer.Deserialize<
                        List<LichSuChamCongModel>>(
                            json,
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            })
                    ?? new List<LichSuChamCongModel>();


                // CHỈ LẤY CHẤM CÔNG CỦA NHÂN VIÊN ĐƯỢC CHỌN

                chamCongs =
                    chamCongs
                        .Where(
                            x =>
                                x.NhanVienId == id)
                        .ToList();
                foreach (var item in chamCongs)
                {
                    item.HinhAnh = NormalizeImageUrl(item.HinhAnh);
                }


                // GỬI THÔNG TIN SANG VIEW

                ViewBag.NhanVienId =
                    nhanVien.NhanVienId;

                ViewBag.HoTen =
                    nhanVien.HoTen;

                ViewBag.TenPhongBan =
                    nhanVien.TenPhongBan;


                return View(
                    "ChiTiet",
                    chamCongs);
            }
            catch (Exception ex)
            {
                ViewBag.Error =
                    "Lỗi: " + ex.Message;

                return View(
                    "ChiTiet",
                    new List<LichSuChamCongModel>());
            }
        }


        // LỊCH SỬ CHẤM CÔNG CỦA CHÍNH MÌNH

        [HttpGet]
        public async Task<IActionResult> CuaToi()
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            try
            {
                // LẤY ID NHÂN VIÊN ĐANG ĐĂNG NHẬP

                var currentNhanVienId =
                    GetNhanVienId();


                if (currentNhanVienId == 0)
                {
                    ViewBag.Error =
                        "Không xác định được nhân viên của tài khoản.";

                    return View(
                        "ChiTiet",
                        new List<LichSuChamCongModel>());
                }


                var client = GetClient();


                // LẤY DANH SÁCH NHÂN VIÊN

                var responseNhanVien =
                    await client.GetAsync(
                        "api/NhanVien");


                if (!responseNhanVien.IsSuccessStatusCode)
                {
                    ViewBag.Error =
                        "Không thể lấy thông tin nhân viên.";

                    return View(
                        "ChiTiet",
                        new List<LichSuChamCongModel>());
                }


                var jsonNhanVien =
                    await responseNhanVien.Content
                        .ReadAsStringAsync();


                var nhanViens =
                    JsonSerializer.Deserialize<
                        List<NhanVienModel>>(
                            jsonNhanVien,
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            })
                    ?? new List<NhanVienModel>();


                // TÌM ĐÚNG NHÂN VIÊN ĐANG ĐĂNG NHẬP

                var nhanVien =
                    nhanViens.FirstOrDefault(
                        x =>
                            x.NhanVienId ==
                            currentNhanVienId);


                if (nhanVien == null)
                {
                    ViewBag.Error =
                        "Không tìm thấy thông tin nhân viên.";

                    return View(
                        "ChiTiet",
                        new List<LichSuChamCongModel>());
                }


                // LẤY LỊCH SỬ CHẤM CÔNG

                var response =
                    await client.GetAsync(
                        "api/ChamCong");


                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error =
                        "Không thể lấy lịch sử chấm công.";

                    return View(
                        "ChiTiet",
                        new List<LichSuChamCongModel>());
                }


                var json =
                    await response.Content
                        .ReadAsStringAsync();


                var data =
                    JsonSerializer.Deserialize<
                        List<LichSuChamCongModel>>(
                            json,
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            })
                    ?? new List<LichSuChamCongModel>();


                // QUAN TRỌNG:
                // CHỈ LẤY CHẤM CÔNG CỦA CHÍNH MÌNH
                data =
                    data
                        .Where(
                            x =>
                                x.NhanVienId ==
                                currentNhanVienId)
                        .ToList();
                foreach (var item in data)
                {
                    item.HinhAnh = NormalizeImageUrl(item.HinhAnh);
                }


                // THÔNG TIN NHÂN VIÊN

                ViewBag.NhanVienId =
                    nhanVien.NhanVienId;

                ViewBag.HoTen =
                    nhanVien.HoTen;

                ViewBag.TenPhongBan =
                    nhanVien.TenPhongBan;


                return View(
                    "ChiTiet",
                    data);
            }
            catch (Exception ex)
            {
                ViewBag.Error =
                    "Lỗi: " + ex.Message;

                return View(
                    "ChiTiet",
                    new List<LichSuChamCongModel>());
            }
        }

    }
}
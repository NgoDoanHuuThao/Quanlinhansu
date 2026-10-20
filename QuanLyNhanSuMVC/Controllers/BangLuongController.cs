using Microsoft.AspNetCore.Mvc;
using QuanLyNhanSuMVC.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
namespace QuanLyNhanSuMVC.Controllers
{
    public class BangLuongController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly JsonSerializerOptions _jsonOptions = new()
        {

            PropertyNameCaseInsensitive = true
        };

        public BangLuongController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // LẤY HTTP CLIENT + JWT
        private HttpClient GetClient()
        {
            var client = _httpClientFactory.CreateClient("QuanLyNhanSuAPI");
            var jwt = HttpContext.Session.GetString("JWT");

            if (!string.IsNullOrEmpty(jwt))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", jwt);
            }

            return client;
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
            return HttpContext.Session.GetString("Role") ?? "";
        }

        // DANH SÁCH BẢNG LƯƠNG
        [HttpGet]
        public async Task<IActionResult> Index(int? thang, int? nam)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            var role = GetRole();

            if (role != "GiamDoc" &&
                role != "TruongPhong" &&
                role != "NhanVien")
                return Forbid();

            int selectedThang = thang ?? 0;
            int selectedNam = nam ?? DateTime.Now.Year;

            if (selectedThang < 0 || selectedThang > 12)
                selectedThang = 0;

            if (selectedNam < 2000 || selectedNam > 2100)
                selectedNam = DateTime.Now.Year;

            ViewBag.Thang = selectedThang;
            ViewBag.Nam = selectedNam;
            ViewBag.Role = role;

            var client = GetClient();
            var bangLuongs = new List<BangLuongModel>();

            // Nhân viên chỉ xem bảng lương của bản thân
            if (role == "NhanVien")
            {
                var response = await client.GetAsync("api/BangLuong/me");

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Không thể lấy bảng lương.";
                    return View(bangLuongs);
                }

                var json = await response.Content.ReadAsStringAsync();

                bangLuongs =
                    JsonSerializer.Deserialize<List<BangLuongModel>>(
                        json, _jsonOptions)
                    ?? new List<BangLuongModel>();
            }
            else
            {
                // Lấy danh sách bảng lương
                var blResponse = await client.GetAsync("api/BangLuong");

                if (!blResponse.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Không thể lấy danh sách bảng lương.";
                    return View(bangLuongs);
                }

                var blJson = await blResponse.Content.ReadAsStringAsync();

                var bangLuongsAll =
                    JsonSerializer.Deserialize<List<BangLuongModel>>(
                        blJson, _jsonOptions)
                    ?? new List<BangLuongModel>();

                if (role == "GiamDoc")
                {
                    bangLuongs = bangLuongsAll;
                }
                else
                {
                    // Trưởng phòng chỉ xem nhân viên thuộc phòng mình
                    var nhanVienIdString =
                        HttpContext.Session.GetString("NhanVienId");

                    if (!int.TryParse(nhanVienIdString, out int nhanVienId))
                    {
                        ViewBag.Error =
                            "Không xác định được nhân viên đang đăng nhập.";
                        return View(bangLuongs);
                    }

                    var nvResponse = await client.GetAsync(
                        "api/NhanVien?pageNumber=1&pageSize=1000");

                    if (!nvResponse.IsSuccessStatusCode)
                    {
                        ViewBag.Error = "Không thể lấy danh sách nhân viên.";
                        return View(bangLuongs);
                    }

                    var nvJson = await nvResponse.Content.ReadAsStringAsync();

                    var nhanViens =
                        JsonSerializer.Deserialize<List<NhanVienModel>>(
                            nvJson, _jsonOptions)
                        ?? new List<NhanVienModel>();

                    var truongPhong = nhanViens.FirstOrDefault(
                        x => x.NhanVienId == nhanVienId);

                    if (truongPhong == null)
                    {
                        ViewBag.Error = "Không tìm thấy thông tin trưởng phòng.";
                        return View(bangLuongs);
                    }

                    int phongBanId = truongPhong.PhongBanId;

                    var nhanVienTrongPhong = nhanViens
                        .Where(x => x.PhongBanId == phongBanId)
                        .Select(x => x.NhanVienId)
                        .ToHashSet();

                    bangLuongs = bangLuongsAll
                        .Where(x => nhanVienTrongPhong.Contains(x.NhanVienId))
                        .ToList();
                }
            }

            // Lọc theo tháng nếu người dùng chọn tháng cụ thể
            if (selectedThang > 0)
            {
                bangLuongs = bangLuongs
                    .Where(x => x.Thang == selectedThang)
                    .ToList();
            }

            // Lọc theo năm
            bangLuongs = bangLuongs
                .Where(x => x.Nam == selectedNam)
                .OrderBy(x => x.Thang)
                .ThenBy(x => x.HoTenNhanVien)
                .ToList();

            return View(bangLuongs);
        }

        // FORM THÊM BẢNG LƯƠNG
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            if (GetRole() != "GiamDoc")
                return Forbid();

            await LoadNhanViens();

            return View(new BangLuongFormModel
            {
                Thang = DateTime.Now.Month,
                Nam = DateTime.Now.Year
            });
        }

        // THÊM BẢNG LƯƠNG
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BangLuongFormModel model)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            if (GetRole() != "GiamDoc")
                return Forbid();

            if (!ModelState.IsValid)
            {
                await LoadNhanViens();
                return View(model);
            }

            var client = GetClient();
            var json = JsonSerializer.Serialize(model);

            var content = new StringContent(
                json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/BangLuong", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["BangLuongSuccess"] =
                    "Thêm bảng lương thành công.";

                return RedirectToAction(nameof(Index));
            }

            var error = await response.Content.ReadAsStringAsync();
            AddApiError(error);

            await LoadNhanViens();
            return View(model);
        }

        // FORM SỬA
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            if (GetRole() != "GiamDoc")
                return Forbid();

            var client = GetClient();
            var response = await client.GetAsync($"api/BangLuong/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();

            var bangLuong =
                JsonSerializer.Deserialize<BangLuongModel>(
                    json, _jsonOptions);

            if (bangLuong == null)
                return NotFound();

            var model = new BangLuongFormModel
            {
                NhanVienId = bangLuong.NhanVienId,
                Thang = bangLuong.Thang,
                Nam = bangLuong.Nam,
                LuongCoBan = bangLuong.LuongCoBan,
                PhuCap = bangLuong.PhuCap,
                Thuong = bangLuong.Thuong
            };

            await LoadNhanViens();
            ViewBag.BangLuongId = bangLuong.BangLuongId;

            return View(model);
        }

        // CẬP NHẬT BẢNG LƯƠNG
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id, BangLuongFormModel model)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            if (GetRole() != "GiamDoc")
                return Forbid();

            if (!ModelState.IsValid)
            {
                await LoadNhanViens();
                ViewBag.BangLuongId = id;
                return View(model);
            }

            var client = GetClient();
            var json = JsonSerializer.Serialize(model);

            var content = new StringContent(
                json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(
                $"api/BangLuong/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["BangLuongSuccess"] =
                    "Cập nhật bảng lương thành công.";

                return RedirectToAction(nameof(Index));
            }

            var error = await response.Content.ReadAsStringAsync();
            AddApiError(error);

            await LoadNhanViens();
            ViewBag.BangLuongId = id;

            return View(model);
        }

        // XÓA BẢNG LƯƠNG
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            if (GetRole() != "GiamDoc")
                return Forbid();

            var client = GetClient();
            var response = await client.DeleteAsync($"api/BangLuong/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["BangLuongSuccess"] =
                    "Xóa bảng lương thành công.";
            }
            else
            {
                TempData["BangLuongError"] =
                    "Không thể xóa bảng lương.";
            }

            return RedirectToAction(nameof(Index));
        }

        // TẠO / TÍNH LƯƠNG CHO TẤT CẢ NHÂN VIÊN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaoTatCa(int thang, int nam)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            if (GetRole() != "GiamDoc")
                return Forbid();

            if (thang < 1 || thang > 12)
            {
                TempData["BangLuongError"] = "Tháng không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            if (nam < 2000 || nam > 2100)
            {
                TempData["BangLuongError"] = "Năm không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var client = GetClient();

            var request = new { thang, nam };
            var json = JsonSerializer.Serialize(request);

            var content = new StringContent(
                json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(
                "api/BangLuong/tao-tat-ca", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["BangLuongSuccess"] =
                    $"Đã tính lương tháng {thang}/{nam} thành công.";


                return RedirectToAction(
                    nameof(Index),
                    new { thang = thang, nam = nam });

            }

            var error = await response.Content.ReadAsStringAsync();

            TempData["BangLuongError"] =
                "Không thể tính lương: " + error;

            return RedirectToAction(nameof(Index));
        }

        // LOAD DANH SÁCH NHÂN VIÊN CHO FORM
        private async Task LoadNhanViens()
        {
            var client = GetClient();

            var response = await client.GetAsync(
                "api/NhanVien?pageNumber=1&pageSize=1000");

            var nhanViens = new List<NhanVienModel>();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                nhanViens =
                    JsonSerializer.Deserialize<List<NhanVienModel>>(
                        json, _jsonOptions)
                    ?? new List<NhanVienModel>();
            }

            ViewBag.NhanViens = nhanViens;
        }

        // HIỂN THỊ LỖI API TRÊN FORM
        private void AddApiError(string error)
        {
            try
            {
                using var document = JsonDocument.Parse(error);
                var root = document.RootElement;

                if (root.ValueKind == JsonValueKind.Object &&
                    root.TryGetProperty("message", out var message))
                {
                    ModelState.AddModelError(
                        string.Empty, message.ToString());
                }
                else
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Không thể thực hiện thao tác.");
                }
            }
            catch
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Không thể thực hiện thao tác.");
            }
        }

        // THỐNG KÊ LƯƠNG THEO THÁNG
        [HttpGet]
        public async Task<IActionResult> ThongKe(int? thang, int? nam)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            if (GetRole() != "GiamDoc")
                return Forbid();

            int selectedThang = thang ?? DateTime.Now.Month;
            int selectedNam = nam ?? DateTime.Now.Year;

            var client = GetClient();

            var response = await client.GetAsync(
                $"api/BangLuong/thong-ke?thang={selectedThang}&nam={selectedNam}");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Không thể lấy dữ liệu thống kê.";

                return View(new ThongKeLuongModel
                {
                    Thang = selectedThang,
                    Nam = selectedNam
                });
            }

            var json = await response.Content.ReadAsStringAsync();

            var model =
                JsonSerializer.Deserialize<ThongKeLuongModel>(
                    json, _jsonOptions)
                ?? new ThongKeLuongModel
                {
                    Thang = selectedThang,
                    Nam = selectedNam
                };

            return View(model);
        }

        // THỐNG KÊ LƯƠNG THEO NĂM
        [HttpGet]
        public async Task<IActionResult> ThongKeNam(int? nam)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            if (GetRole() != "GiamDoc")
                return Forbid();

            int selectedNam = nam ?? DateTime.Now.Year;

            if (selectedNam < 2000 || selectedNam > 2100)
                selectedNam = DateTime.Now.Year;

            var client = GetClient();

            var response = await client.GetAsync(
                $"api/BangLuong/thong-ke-nam?nam={selectedNam}");

            var data = new List<ThongKeLuongNamModel>();

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                data =
                    JsonSerializer.Deserialize<List<ThongKeLuongNamModel>>(
                        json, _jsonOptions)
                    ?? new List<ThongKeLuongNamModel>();
            }
            else
            {
                ViewBag.Error =
                    "Không thể lấy dữ liệu thống kê theo năm.";
            }

            // ĐẢM BẢO CÓ ĐỦ 12 THÁNG
            var fullYearData = new List<ThongKeLuongNamModel>();

            for (int thang = 1; thang <= 12; thang++)
            {
                var item = data.FirstOrDefault(x => x.Thang == thang);

                if (item != null)
                {
                    fullYearData.Add(item);
                }
                else
                {
                    fullYearData.Add(new ThongKeLuongNamModel
                    {
                        Nam = selectedNam,
                        Thang = thang,
                        SoNhanVien = 0,
                        TongLuongCoBan = 0,
                        TongPhuCap = 0,
                        TongThuong = 0,
                        TongQuyLuong = 0
                    });
                }
            }

            ViewBag.Nam = selectedNam;

            return View(fullYearData);
        }
    }

}


using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using QuanLyNhanSuMVC.Models;
namespace QuanLyNhanSuMVC.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public TaiKhoanController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

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

        private bool IsLoggedIn()
        {
            return !string.IsNullOrEmpty(
                HttpContext.Session.GetString("JWT"));
        }

        private bool IsGiamDoc()
        {
            var role = HttpContext.Session.GetString("Role");

            return role == "GiamDoc";
        }

        // DANH SÁCH TÀI KHOẢN
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            if (!IsGiamDoc())
                return Forbid();

            var client = GetClient();

            var response = await client.GetAsync("api/TaiKhoan");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Không thể lấy danh sách tài khoản.";
                return View(new List<TaiKhoanModel>());
            }

            var json = await response.Content.ReadAsStringAsync();

            var taiKhoans =
                JsonSerializer.Deserialize<List<TaiKhoanModel>>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            return View(taiKhoans ?? new List<TaiKhoanModel>());
        }

        // FORM THÊM TÀI KHOẢN
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            if (!IsGiamDoc())
                return Forbid();

            var client = GetClient();

            // Lấy tất cả nhân viên
            var response =
                await client.GetAsync(
                    "api/NhanVien?pageNumber=1&pageSize=1000");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Không thể lấy danh sách nhân viên.";
                return View(new AddTaiKhoanModel());
            }

            var json = await response.Content.ReadAsStringAsync();

            var nhanViens =
                JsonSerializer.Deserialize<List<NhanVienModel>>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            // Lấy danh sách tài khoản hiện tại
            var accountResponse =
                await client.GetAsync("api/TaiKhoan");

            var taiKhoans = new List<TaiKhoanModel>();

            if (accountResponse.IsSuccessStatusCode)
            {
                var accountJson =
                    await accountResponse.Content.ReadAsStringAsync();

                taiKhoans =
                    JsonSerializer.Deserialize<List<TaiKhoanModel>>(
                        accountJson,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        })
                    ?? new List<TaiKhoanModel>();
            }

            // Chỉ hiển thị nhân viên chưa có tài khoản
            var nhanVienDaCoTaiKhoan =
                taiKhoans
                    .Where(x => x.NhanVienId.HasValue)
                    .Select(x => x.NhanVienId!.Value)
                    .ToHashSet();

            var nhanVienChuaCoTaiKhoan =
                (nhanViens ?? new List<NhanVienModel>())
                    .Where(x => !nhanVienDaCoTaiKhoan.Contains(x.NhanVienId))
                    .ToList();

            ViewBag.NhanViens = nhanVienChuaCoTaiKhoan;

            return View(new AddTaiKhoanModel());
        }

        // THÊM TÀI KHOẢN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddTaiKhoanModel model)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            if (!IsGiamDoc())
                return Forbid();

            if (!ModelState.IsValid)
            {
                await LoadNhanVienChuaCoTaiKhoan();
                return View(model);
            }

            var client = GetClient();

            var json = JsonSerializer.Serialize(model);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var response =
                await client.PostAsync(
                    "api/TaiKhoan",
                    content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] =
                    "Thêm tài khoản thành công.";

                return RedirectToAction(nameof(Index));
            }

            var error =
                await response.Content.ReadAsStringAsync();

            try
            {
                var errorObject =
                    JsonSerializer.Deserialize<Dictionary<string, string>>(
                        error);

                if (errorObject != null &&
                    errorObject.ContainsKey("message"))
                {
                    ModelState.AddModelError(
                        "",
                        errorObject["message"]);
                }
                else
                {
                    ModelState.AddModelError(
                        "",
                        "Không thể thêm tài khoản.");
                }
            }
            catch
            {
                ModelState.AddModelError(
                    "",
                    "Không thể thêm tài khoản.");
            }

            await LoadNhanVienChuaCoTaiKhoan();

            return View(model);
        }

        // LOAD NHÂN VIÊN CHƯA CÓ TK
        private async Task LoadNhanVienChuaCoTaiKhoan()
        {
            var client = GetClient();

            var response =
                await client.GetAsync(
                    "api/NhanVien?pageNumber=1&pageSize=1000");

            var nhanViens = new List<NhanVienModel>();

            if (response.IsSuccessStatusCode)
            {
                var json =
                    await response.Content.ReadAsStringAsync();

                nhanViens =
                    JsonSerializer.Deserialize<List<NhanVienModel>>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        })
                    ?? new List<NhanVienModel>();
            }

            var accountResponse =
                await client.GetAsync("api/TaiKhoan");

            var taiKhoans = new List<TaiKhoanModel>();

            if (accountResponse.IsSuccessStatusCode)
            {
                var json =
                    await accountResponse.Content.ReadAsStringAsync();

                taiKhoans =
                    JsonSerializer.Deserialize<List<TaiKhoanModel>>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        })
                    ?? new List<TaiKhoanModel>();
            }

            var ids =
                taiKhoans
                    .Where(x => x.NhanVienId.HasValue)
                    .Select(x => x.NhanVienId!.Value)
                    .ToHashSet();

            ViewBag.NhanViens =
                nhanViens
                    .Where(x => !ids.Contains(x.NhanVienId))
                    .ToList();
        }
    }
}


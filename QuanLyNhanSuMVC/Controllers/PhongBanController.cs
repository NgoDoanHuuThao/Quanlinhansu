using Microsoft.AspNetCore.Mvc;
using QuanLyNhanSuMVC.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace QuanLyNhanSuMVC.Controllers
{
    public class PhongBanController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public PhongBanController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient GetClient()
        {
            var client = _httpClientFactory.CreateClient("QuanLyNhanSuAPI");

            var token = HttpContext.Session.GetString("JWT");

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        private bool IsLoggedIn()
        {
            return !string.IsNullOrEmpty(
                HttpContext.Session.GetString("JWT"));
        }

        // DANH SÁCH PHÒNG BAN
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            try
            {
                var client = GetClient();

                var response = await client.GetAsync("api/PhongBan");

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Không thể lấy danh sách phòng ban.";
                    return View(new List<PhongBanModel>());
                }

                var json = await response.Content.ReadAsStringAsync();

                var data = JsonSerializer.Deserialize<List<PhongBanModel>>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                return View(data ?? new List<PhongBanModel>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new List<PhongBanModel>());
            }
        }

        // FORM THÊM
        [HttpGet]
        public IActionResult Create()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            return View();
        }

        // XỬ LÝ THÊM
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PhongBanModel model)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var client = GetClient();

                var json = JsonSerializer.Serialize(new
                {
                    tenPhongBan = model.TenPhongBan,
                    moTa = model.MoTa
                });

                var content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json");

                var response = await client.PostAsync(
                    "api/PhongBan",
                    content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                var error = await response.Content.ReadAsStringAsync();

                ViewBag.Error = "Thêm phòng ban thất bại: " + error;

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(model);
            }
        }

        // FORM SỬA
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            try
            {
                var client = GetClient();

                var response = await client.GetAsync(
                    $"api/PhongBan/{id}");

                if (!response.IsSuccessStatusCode)
                    return NotFound();

                var json = await response.Content.ReadAsStringAsync();

                var model = JsonSerializer.Deserialize<PhongBanModel>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (model == null)
                    return NotFound();

                return View(model);
            }
            catch
            {
                return NotFound();
            }
        }

        // XỬ LÝ SỬA
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            PhongBanModel model)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var client = GetClient();

                var json = JsonSerializer.Serialize(new
                {
                    phongBanId = id,
                    tenPhongBan = model.TenPhongBan,
                    moTa = model.MoTa
                });

                var content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json");

                var response = await client.PutAsync(
                    $"api/PhongBan/{id}",
                    content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                var error = await response.Content.ReadAsStringAsync();

                ViewBag.Error = "Cập nhật thất bại: " + error;

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(model);
            }
        }

        // XÓA
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            try
            {
                var client = GetClient();

                var response = await client.DeleteAsync(
                    $"api/PhongBan/{id}");

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }
    }
}

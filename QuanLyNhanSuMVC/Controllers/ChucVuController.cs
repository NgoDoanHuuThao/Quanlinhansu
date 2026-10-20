using Microsoft.AspNetCore.Mvc;
using QuanLyNhanSuMVC.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
namespace QuanLyNhanSuMVC.Controllers
{
    public class ChucVuController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ChucVuController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient GetClient()
        {
            var client =
                _httpClientFactory.CreateClient("QuanLyNhanSuAPI");

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

        // DANH SÁCH
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            try
            {
                var client = GetClient();

                var response =
                    await client.GetAsync("api/ChucVu");

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error =
                        "Không thể lấy danh sách chức vụ.";

                    return View(new List<ChucVuModel>());
                }

                var json =
                    await response.Content.ReadAsStringAsync();

                var data =
                    JsonSerializer.Deserialize<List<ChucVuModel>>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                return View(
                    data ?? new List<ChucVuModel>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;

                return View(
                    new List<ChucVuModel>());
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

        // THÊM
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            ChucVuModel model)
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
                    tenChucVu = model.TenChucVu,
                    moTa = model.MoTa
                });

                var content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json");

                var response =
                    await client.PostAsync(
                        "api/ChucVu",
                        content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                var error =
                    await response.Content.ReadAsStringAsync();

                ViewBag.Error =
                    "Thêm chức vụ thất bại: " + error;

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

                var response =
                    await client.GetAsync(
                        $"api/ChucVu/{id}");

                if (!response.IsSuccessStatusCode)
                    return NotFound();

                var json =
                    await response.Content.ReadAsStringAsync();

                var model =
                    JsonSerializer.Deserialize<ChucVuModel>(
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

        // CẬP NHẬT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            ChucVuModel model)
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
                    chucVuId = id,
                    tenChucVu = model.TenChucVu,
                    moTa = model.MoTa
                });

                var content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json");

                var response =
                    await client.PutAsync(
                        $"api/ChucVu/{id}",
                        content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                var error =
                    await response.Content.ReadAsStringAsync();

                ViewBag.Error =
                    "Cập nhật thất bại: " + error;

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

                await client.DeleteAsync(
                    $"api/ChucVu/{id}");

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }
    }
}

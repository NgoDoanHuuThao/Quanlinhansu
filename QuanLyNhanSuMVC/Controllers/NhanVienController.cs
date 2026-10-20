using Microsoft.AspNetCore.Mvc;
using QuanLyNhanSuMVC.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
namespace QuanLyNhanSuMVC.Controllers
{
    public class NhanVienController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public NhanVienController(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

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

        private bool IsLoggedIn()
        {
            return !string.IsNullOrEmpty(
                HttpContext.Session.GetString("JWT"));
        }

        // DANH SÁCH NHÂN VIÊN
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            try
            {
                var client = GetClient();

                var response =
                    await client.GetAsync("api/NhanVien");

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error =
                        "Không thể lấy danh sách nhân viên.";

                    return View(
                        new List<NhanVienModel>());
                }

                var json =
                    await response.Content
                        .ReadAsStringAsync();

                var employees =
                    JsonSerializer.Deserialize<
                        List<NhanVienModel>>(
                            json,
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });

                return View(
                    employees ??
                    new List<NhanVienModel>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi: " + ex.Message;

                return View(
                    new List<NhanVienModel>());
            }
        }

        // TRANG THÊM NHÂN VIÊN
        [HttpGet]
        public IActionResult Create()
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            return View();
        }

        // THÊM NHÂN VIÊN
        [HttpPost]
        public async Task<IActionResult> Create(
            NhanVienModel model)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            try
            {
                var client = GetClient();

                var json =
                    JsonSerializer.Serialize(model);

                var content =
                    new StringContent(
                        json,
                        Encoding.UTF8,
                        "application/json");

                var response =
                    await client.PostAsync(
                        "api/NhanVien",
                        content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(
                        "Index");
                }

                var error =
                    await response.Content
                        .ReadAsStringAsync();

                ViewBag.Error =
                    "Không thêm được nhân viên: "
                    + error;

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error =
                    "Lỗi: " + ex.Message;

                return View(model);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var client = GetClient();

                var response = await client.GetAsync(
                    $"api/NhanVien/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }

                var json = await response.Content
                    .ReadAsStringAsync();

                var employee =
                    JsonSerializer.Deserialize<NhanVienModel>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                if (employee == null)
                {
                    return NotFound();
                }

                return View(employee);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi: " + ex.Message;
                return View();
            }
        }


        // LƯU SỬA NHÂN VIÊN
        [HttpPost]
        public async Task<IActionResult> Edit(
            int id,
            NhanVienModel model)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var client = GetClient();

                var json = JsonSerializer.Serialize(model);

                var content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json");

                var response = await client.PutAsync(
                    $"api/NhanVien/{id}",
                    content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }

                var error = await response.Content
                    .ReadAsStringAsync();

                ViewBag.Error =
                    "Không thể cập nhật nhân viên: " + error;

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi: " + ex.Message;

                return View(model);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var client = GetClient();

                var response = await client.GetAsync(
                    $"api/NhanVien/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }

                var json = await response.Content
                    .ReadAsStringAsync();

                var employee =
                    JsonSerializer.Deserialize<NhanVienModel>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                return View(employee);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi: " + ex.Message;
                return View();
            }
        }


        // THỰC HIỆN XÓA
        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsLoggedIn())
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var client = GetClient();

                var response = await client.DeleteAsync(
                    $"api/NhanVien/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }

                var error = await response.Content
                    .ReadAsStringAsync();

                TempData["Error"] =
                    "Không thể xóa nhân viên: " + error;

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "Lỗi: " + ex.Message;

                return RedirectToAction("Index");
            }
        }
    }
}

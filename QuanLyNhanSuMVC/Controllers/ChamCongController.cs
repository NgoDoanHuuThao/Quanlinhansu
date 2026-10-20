using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace QuanLyNhanSuMVC.Controllers
{
    public class ChamCongController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ChamCongController(IHttpClientFactory httpClientFactory)
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

        [HttpGet]
        public IActionResult Index()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            return RedirectToAction(nameof(Create));
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
    IFormFile? HinhAnh,
    string? ViTri,
    string? Latitude,
    string? Longitude)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login", "Account");

            if (HinhAnh == null || HinhAnh.Length == 0)
            {
                ViewBag.Error = "Vui lòng chọn ảnh chấm công.";
                return View();
            }

            var allowedTypes = new[]
            {
        "image/jpeg",
        "image/png"
    };

            if (!allowedTypes.Contains(HinhAnh.ContentType))
            {
                ViewBag.Error = "Chỉ chấp nhận ảnh JPG hoặc PNG.";
                return View();
            }

            if (HinhAnh.Length > 5 * 1024 * 1024)
            {
                ViewBag.Error = "Ảnh không được vượt quá 5 MB.";
                return View();
            }

            if (string.IsNullOrWhiteSpace(ViTri) ||
                string.IsNullOrWhiteSpace(Latitude) ||
                string.IsNullOrWhiteSpace(Longitude))
            {
                ViewBag.Error =
                    "Vị trí hoặc tọa độ GPS đang bị trống. Vui lòng tải lại trang và cho phép truy cập vị trí.";
                return View();
            }

            try
            {
                var client = GetClient();

                using var form = new MultipartFormDataContent();

                await using var stream = HinhAnh.OpenReadStream();
                using var fileContent = new StreamContent(stream);

                fileContent.Headers.ContentType =
                    new MediaTypeHeaderValue(HinhAnh.ContentType);

                form.Add(fileContent, "HinhAnh", HinhAnh.FileName);
                form.Add(new StringContent(ViTri), "ViTri");
                form.Add(new StringContent(Latitude), "Latitude");
                form.Add(new StringContent(Longitude), "Longitude");

                var response = await client.PostAsync(
                    "api/ChamCong",
                    form);

                var responseText =
                    await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Chấm công thành công!";
                    return RedirectToAction(nameof(Create));
                }

                ViewBag.Error =
                    $"Lưu chấm công thất bại ({(int)response.StatusCode}): {responseText}";

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi khi chấm công: " + ex.Message;
                return View();
            }
        }
    }
}

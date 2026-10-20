using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using QuanLyNhanSuMVC.Models;

namespace QuanLyNhanSuMVC.Controllers
{
    public class ThongBaoController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ThongBaoController(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient TaoHttpClient()
        {
            var client = _httpClientFactory.CreateClient();

            client.BaseAddress = new Uri("https://localhost:7151/");

            var token = HttpContext.Session.GetString("JWT");

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        // GET: /ThongBao
        public async Task<IActionResult> Index()
        {
            using var client = TaoHttpClient();

            try
            {
                var response =
                    await client.GetAsync("api/ThongBao");

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Loi =
                        $"API trả về lỗi {(int)response.StatusCode}.";
                    return View(new List<ThongBaoModel>());
                }

                var danhSach =
                    await response.Content
                        .ReadFromJsonAsync<List<ThongBaoModel>>();

                return View(danhSach ?? new List<ThongBaoModel>());
            }
            catch (HttpRequestException)
            {
                ViewBag.Loi =
                    "Không kết nối được API. Hãy kiểm tra project QuanLyNhanSu.";
                return View(new List<ThongBaoModel>());
            }
        }

        // POST: /ThongBao/DaDoc
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DaDoc(int id)
        {
            using var client = TaoHttpClient();

            var response =
                await client.PutAsync(
                    $"api/ThongBao/{id}/da-doc", null);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Loi"] =
                    "Không thể cập nhật trạng thái đã đọc.";
            }
            else
            {
                TempData["ThongBao"] =
                    "Đã đánh dấu thông báo đã đọc.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /ThongBao/Tao
        [HttpGet]
        public IActionResult Tao()
        {
            return View(new ThongBaoModel());
        }

        // POST: /ThongBao/Tao
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Tao(ThongBaoModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var client = TaoHttpClient();

            var request = new
            {
                model.TieuDe,
                model.NoiDung,
                model.DoiTuongNhan,
                PhongBanId = model.DoiTuongNhan == "ALL"
                    ? (int?)null
                    : model.PhongBanId,
                model.NgayHetHan
            };

            try
            {
                var response = await client.PostAsJsonAsync(
                    "api/ThongBao", request);

                if (!response.IsSuccessStatusCode)
                {
                    var noiDungLoi =
                        await response.Content.ReadAsStringAsync();

                    ModelState.AddModelError(
                        string.Empty,
                        $"Gửi thông báo thất bại: {noiDungLoi}");

                    return View(model);
                }

                TempData["ThongBao"] =
                    "Gửi thông báo thành công.";

                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Không kết nối được API.");
                return View(model);
            }
        }
    }
}

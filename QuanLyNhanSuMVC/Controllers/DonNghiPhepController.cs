using Microsoft.AspNetCore.Mvc;
using QuanLyNhanSuMVC.Models;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace QuanLyNhanSuMVC.Controllers
{
    public class DonNghiPhepController : Controller
    {
        // Hiển thị danh sách đơn nghỉ phép
        public async Task<IActionResult> Index(string? trangThai)
        {
            var jwt = HttpContext.Session.GetString("JWT");

            if (string.IsNullOrWhiteSpace(jwt))
            {
                return RedirectToAction("Login", "Account");
            }

            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7151/");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwt);

            try
            {
                var response =
                    await client.GetAsync("api/DonNghiPhep");

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Loi =
                        $"Không tải được danh sách đơn nghỉ phép. Mã lỗi: {(int)response.StatusCode}";

                    return View(new List<DonNghiPhepModel>());
                }

                var danhSach = await response.Content
    .ReadFromJsonAsync<List<DonNghiPhepModel>>()
    ?? new List<DonNghiPhepModel>();

                if (!string.IsNullOrWhiteSpace(trangThai))
                {
                    danhSach = danhSach
                        .Where(x => x.TrangThai == trangThai)
                        .ToList();
                }

                ViewBag.TrangThaiDangLoc = trangThai;

                return View(danhSach);
            }
            catch (HttpRequestException)
            {
                ViewBag.Loi =
                    "Không kết nối được API quản lý nhân sự.";

                return View(new List<DonNghiPhepModel>());
            }
        }

        // Mở form tạo đơn nghỉ phép
        [HttpGet]
        public IActionResult TaoDon()
        {
            var jwt = HttpContext.Session.GetString("JWT");

            if (string.IsNullOrWhiteSpace(jwt))
            {
                return RedirectToAction("Login", "Account");
            }

            return View(new DonNghiPhepModel
            {
                NgayBatDauNghi = DateTime.Today.AddDays(1),
                NgayKetThucNghi = DateTime.Today.AddDays(1)
            });
        }

        // Gửi đơn nghỉ phép lên API
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TaoDon(
            DonNghiPhepModel model)
        {
            if (model.NgayBatDauNghi.Date < DateTime.Today)
            {
                ModelState.AddModelError(
                    nameof(model.NgayBatDauNghi),
                    "Ngày bắt đầu nghỉ không được ở quá khứ.");
            }

            if (model.NgayKetThucNghi.Date <
                model.NgayBatDauNghi.Date)
            {
                ModelState.AddModelError(
                    nameof(model.NgayKetThucNghi),
                    "Ngày kết thúc không được trước ngày bắt đầu.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var jwt = HttpContext.Session.GetString("JWT");

            if (string.IsNullOrWhiteSpace(jwt))
            {
                return RedirectToAction("Login", "Account");
            }

            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7151/");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwt);

            var request = new
            {
                ngayBatDauNghi =
                    model.NgayBatDauNghi.ToString("yyyy-MM-dd"),

                ngayKetThucNghi =
                    model.NgayKetThucNghi.ToString("yyyy-MM-dd"),

                lyDoNghi = model.LyDoNghi.Trim()
            };

            try
            {
                var response = await client.PostAsJsonAsync(
                    "api/DonNghiPhep", request);

                if (response.IsSuccessStatusCode)
                {
                    TempData["ThongBao"] =
                        "Tạo đơn nghỉ phép thành công!";

                    return RedirectToAction(nameof(Index));
                }

                var error =
                    await response.Content.ReadAsStringAsync();

                ModelState.AddModelError(
                    string.Empty,
                    $"API trả về lỗi {(int)response.StatusCode}: {error}");

                return View(model);
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Không kết nối được API. Hãy kiểm tra API có đang chạy không.");

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Duyet(
            int id,
            string trangThai,
            string? ghiChuDuyet,
            bool huongLuong = true)
        {
            var jwt = HttpContext.Session.GetString("JWT");

            if (string.IsNullOrWhiteSpace(jwt))
            {
                return RedirectToAction("Login", "Account");
            }

            if (trangThai != "DaDuyet" && trangThai != "TuChoi")
            {
                TempData["ThongBaoLoi"] = "Trạng thái không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            using var client = new HttpClient();

            client.BaseAddress = new Uri("https://localhost:7151/");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwt);

            var request = new
            {
                trangThai = trangThai,
                ghiChuDuyet = ghiChuDuyet,
                huongLuong = huongLuong
            };

            try
            {
                var response = await client.PutAsJsonAsync(
                    $"api/DonNghiPhep/{id}/duyet",
                    request);

                if (response.IsSuccessStatusCode)
                {
                    TempData["ThongBao"] =
                        trangThai == "DaDuyet"
                            ? (huongLuong
                                ? "Đã duyệt đơn nghỉ phép hưởng lương."
                                : "Đã duyệt đơn nghỉ phép không hưởng lương.")
                            : "Đã từ chối đơn nghỉ phép.";
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();

                    TempData["ThongBaoLoi"] =
                        $"Không thể xử lý đơn. Mã lỗi {(int)response.StatusCode}: {error}";
                }
            }
            catch (HttpRequestException)
            {
                TempData["ThongBaoLoi"] =
                    "Không kết nối được API quản lý nhân sự.";
            }

            return RedirectToAction(nameof(Index));
        }

        // Xem chi tiết đơn nghỉ phép
        [HttpGet]
public async Task<IActionResult> ChiTiet(int id)
        {
            var jwt = HttpContext.Session.GetString("JWT");

            if (string.IsNullOrWhiteSpace(jwt))
            {
                return RedirectToAction("Login", "Account");
            }

            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7151/");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", jwt);

            try
            {
                var response = await client.GetAsync(
                    $"api/DonNghiPhep/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    TempData["ThongBaoLoi"] =
                        $"Không tải được chi tiết đơn nghỉ phép. Mã lỗi: {(int)response.StatusCode}";

                    return RedirectToAction(nameof(Index));
                }

                var don = await response.Content
                    .ReadFromJsonAsync<DonNghiPhepModel>();

                if (don == null)
                {
                    TempData["ThongBaoLoi"] =
                        "Không tìm thấy thông tin đơn nghỉ phép.";

                    return RedirectToAction(nameof(Index));
                }

                return View(don);
            }
            catch (HttpRequestException)
            {
                TempData["ThongBaoLoi"] =
                    "Không kết nối được API quản lý nhân sự.";

                return RedirectToAction(nameof(Index));
            }
        }

    }
}

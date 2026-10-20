using Microsoft.AspNetCore.Mvc;
using QuanLyNhanSuMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuanLyNhanSuMVC.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DashboardController(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient GetClient()
        {
            var client = _httpClientFactory
                .CreateClient("QuanLyNhanSuAPI");

            var jwt = HttpContext.Session.GetString("JWT");

            if (!string.IsNullOrEmpty(jwt))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", jwt);
            }

            return client;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Kiểm tra đăng nhập
            var jwt = HttpContext.Session.GetString("JWT");

            if (string.IsNullOrEmpty(jwt))
            {
                return RedirectToAction("Login", "Account");
            }

            // Chỉ Giám đốc được xem Dashboard
            var role = HttpContext.Session.GetString("Role");

            if (role != "GiamDoc")
            {
                return RedirectToAction("Index", "Home");
            }

            var client = GetClient();
            var model = new DashboardModel();

            int thangHienTai = DateTime.Now.Month;
            int namHienTai = DateTime.Now.Year;

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            // 1. Đếm nhân viên
            try
            {
                var response = await client.GetAsync(
                    "api/NhanVien?pageNumber=1&pageSize=1000");

                if (response.IsSuccessStatusCode)
                {
                    var json =
                        await response.Content.ReadAsStringAsync();

                    var nhanViens =
                        JsonSerializer.Deserialize<List<NhanVienModel>>(
                            json, jsonOptions);

                    model.TongNhanVien = nhanViens?.Count ?? 0;
                }
                else
                {
                    ViewBag.LoiNhanVien =
                        $"Không thể tải nhân viên. HTTP {(int)response.StatusCode}";
                }
            }
            catch
            {
                ViewBag.LoiNhanVien =
                    "Có lỗi khi tải dữ liệu nhân viên.";
            }

            // 2. Đếm tài khoản
            try
            {
                var response = await client.GetAsync("api/TaiKhoan");

                if (response.IsSuccessStatusCode)
                {
                    var json =
                        await response.Content.ReadAsStringAsync();

                    var taiKhoans =
                        JsonSerializer.Deserialize<List<TaiKhoanModel>>(
                            json, jsonOptions);

                    model.TongTaiKhoan = taiKhoans?.Count ?? 0;
                }
                else
                {
                    ViewBag.LoiTaiKhoan =
                        $"Không thể tải tài khoản. HTTP {(int)response.StatusCode}";
                }
            }
            catch
            {
                ViewBag.LoiTaiKhoan =
                    "Có lỗi khi tải dữ liệu tài khoản.";
            }

            // 3. Khởi tạo đủ 12 tháng
            for (int thang = 1; thang <= 12; thang++)
            {
                model.ThangLabels.Add($"Tháng {thang}");
                model.QuyLuongData.Add(0);
            }

            // 4. Lấy thống kê quỹ lương cả năm
            try
            {
                var response = await client.GetAsync(
                    $"api/BangLuong/thong-ke-nam?nam={namHienTai}");

                if (response.IsSuccessStatusCode)
                {
                    var json =
                        await response.Content.ReadAsStringAsync();

                    var data =
                        JsonSerializer.Deserialize<
                            List<ThongKeLuongNamModel>>(
                                json, jsonOptions)
                        ?? new List<ThongKeLuongNamModel>();

                    foreach (var item in data)
                    {
                        if (item.Thang < 1 || item.Thang > 12)
                        {
                            continue;
                        }

                        double quyLuong = item.TongQuyLuong;

                        model.QuyLuongData[item.Thang - 1] =
                            quyLuong;

                        if (item.Thang == thangHienTai)
                        {
                            model.TongQuyLuong = quyLuong;
                        }
                    }
                }
                else
                {
                    ViewBag.LoiLuong =
                        $"Không thể tải thống kê lương. HTTP {(int)response.StatusCode}";
                }
            }
            catch
            {
                ViewBag.LoiLuong =
                    "Có lỗi khi tải thống kê lương.";
            }

            ViewBag.Thang = thangHienTai;
            ViewBag.Nam = namHienTai;

            return View(model);
        }
    }
    
}

using Microsoft.AspNetCore.Mvc;
using QuanLyNhanSuMVC.Models;
using System.Text;
using System.Text.Json;

namespace QuanLyNhanSuMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // LOGIN - GET

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        // LOGIN - POST

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var client =
                    _httpClientFactory.CreateClient(
                        "QuanLyNhanSuAPI");


                // GỬI USERNAME + PASSWORD LÊN API

                var json = JsonSerializer.Serialize(new
                {
                    tenDangNhap = model.TenDangNhap,
                    matKhau = model.MatKhau
                });

                var content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json");


                var response =
                    await client.PostAsync(
                        "api/TaiKhoan/login",
                        content);


                var responseText =
                    await response.Content.ReadAsStringAsync();


                // LOGIN THẤT BẠI

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error =
                        "Tên đăng nhập hoặc mật khẩu không đúng.";

                    return View(model);
                }


                // ĐỌC RESPONSE API

                using var document =
                    JsonDocument.Parse(responseText);

                var root = document.RootElement;

                string token = "";


                if (root.TryGetProperty(
                    "token",
                    out var tokenProperty))
                {
                    token =
                        tokenProperty.GetString() ?? "";
                }


                // KHÔNG CÓ TOKEN

                if (string.IsNullOrEmpty(token))
                {
                    ViewBag.Error =
                        "API không trả về JWT.";

                    return View(model);
                }


                // LƯU JWT

                HttpContext.Session.SetString(
                    "JWT",
                    token);


                // ĐỌC JWT

                var parts = token.Split('.');


                if (parts.Length >= 2)
                {
                    var payload = parts[1]
                        .Replace('-', '+')
                        .Replace('_', '/');


                    // JWT Base64 có thể thiếu =
                    switch (payload.Length % 4)
                    {
                        case 2:
                            payload += "==";
                            break;

                        case 3:
                            payload += "=";
                            break;
                    }


                    var bytes =
                        Convert.FromBase64String(payload);


                    var payloadJson =
                        Encoding.UTF8.GetString(bytes);


                    using var jwtDocument =
                        JsonDocument.Parse(payloadJson);


                    var jwtRoot =
                        jwtDocument.RootElement;


                    // USERNAME

                    string tenDangNhap =
                        model.TenDangNhap;


                    if (jwtRoot.TryGetProperty(
                        "unique_name",
                        out var usernameProperty))
                    {
                        tenDangNhap =
                            usernameProperty.GetString()
                            ?? model.TenDangNhap;
                    }


                    // ROLE

                    string role = "";


                    if (jwtRoot.TryGetProperty(
                        "role",
                        out var roleProperty))
                    {
                        role =
                            roleProperty.GetString()
                            ?? "";
                    }
                    else if (jwtRoot.TryGetProperty(
                        "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                        out var roleClaimProperty))
                    {
                        role =
                            roleClaimProperty.GetString()
                            ?? "";
                    }


                    // NHÂN VIÊN ID

                    int nhanVienId = 0;


                    if (jwtRoot.TryGetProperty(
                        "NhanVienId",
                        out var nhanVienIdProperty))
                    {
                        // Trường hợp JWT lưu số
                        if (nhanVienIdProperty.ValueKind ==
                            JsonValueKind.Number)
                        {
                            nhanVienId =
                                nhanVienIdProperty.GetInt32();
                        }

                        // Trường hợp JWT lưu chuỗi
                        else if (nhanVienIdProperty.ValueKind ==
                                 JsonValueKind.String)
                        {
                            int.TryParse(
                                nhanVienIdProperty.GetString(),
                                out nhanVienId);
                        }
                    }


                    // LƯU SESSION

                    HttpContext.Session.SetString(
                        "TenDangNhap",
                        tenDangNhap);


                    HttpContext.Session.SetString(
                        "Role",
                        role);


                    // LƯU NHÂN VIÊN ID

                    if (nhanVienId > 0)
                    {
                        HttpContext.Session.SetString(
                            "NhanVienId",
                            nhanVienId.ToString());
                    }
                }

                // VỀ TRANG HOME

                return RedirectToAction(
                    "Index",
                    "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Error =
                    "Lỗi kết nối API: " + ex.Message;

                return View(model);
            }
        }


        // LOGOUT

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction(
                "Login",
                "Account");
        }
    }
}

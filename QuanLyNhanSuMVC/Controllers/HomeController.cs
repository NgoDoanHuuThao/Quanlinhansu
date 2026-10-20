using Microsoft.AspNetCore.Mvc;

namespace QuanLyNhanSuMVC.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            // Kiểm tra đăng nhập
            var jwt = HttpContext.Session.GetString("JWT");

            if (string.IsNullOrEmpty(jwt))
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy thông tin từ Session
            ViewBag.TenDangNhap =
                HttpContext.Session.GetString("TenDangNhap") ?? "";

            ViewBag.Role =
                HttpContext.Session.GetString("Role") ?? "";

            return View();
        }
    }
}

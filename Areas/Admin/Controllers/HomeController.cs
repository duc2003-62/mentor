// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Logging;

// namespace mentor.Areas.Admin.Controllers
// {
//     [Area("Admin")]
//     public class HomeController : Controller
//     {
//         public IActionResult Index()
//         {
//             if (HttpContext.Session.GetInt32("user") == null)
//                 return RedirectToAction("Index", "DangNhap", new { Area = "Admin" });
//             return View();
//         }
//     }
// }

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace mentor.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Kiểm tra nếu người dùng chưa đăng nhập
            if (HttpContext.Session.GetInt32("user") == null)
                return RedirectToAction("Index", "DangNhap", new { Area = "Admin" });

            // Lấy vai trò người dùng từ session
            var role = HttpContext.Session.GetString("role");

            // Kiểm tra vai trò và phân quyền chức năng cho người dùng
            ViewBag.CanAccessDiemDanh = false;
            ViewBag.CanAccessThongKe = false;

            switch (role)
            {
                case "Quản trị viên":
                    // Admin không có quyền vào điểm danh và thống kê
                    ViewBag.CanAccessDiemDanh = false;
                    ViewBag.CanAccessThongKe = false;
                    break;
                case "Giảng viên":
                    // Giảng viên có quyền vào điểm danh
                    ViewBag.CanAccessDiemDanh = true;
                    break;
                case "Sinh viên":
                    // Sinh viên có quyền vào thống kê
                    ViewBag.CanAccessThongKe = true;
                    break;
                default:
                    ViewBag.CanAccessDiemDanh = false;
                    ViewBag.CanAccessThongKe = false;
                    break;
            }

            return View();
        }
    }
}

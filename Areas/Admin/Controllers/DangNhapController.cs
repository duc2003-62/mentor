using System;
using System.Linq;
using mentor.Areas.Admin.Models;
using mentor.Models;
using mentor.Utilities;
using Microsoft.AspNetCore.Mvc;
namespace mentor.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DangNhapController : Controller
    {
        private readonly DataContext _context;

        public DangNhapController(DataContext context)
        {
            _context = context;
        }

        // Phương thức GET để hiển thị giao diện đăng nhập
        [HttpGet]
        public IActionResult Index()
        {
            // Xóa thông báo lỗi cũ nếu có
            TempData["ErrorMessage"] = string.Empty;
            return View();
        }
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public IActionResult Index(mentor.Models.tblNguoiDung user)
        // {
        //     // Kiểm tra đầu vào hợp lệ
        //     if (user == null || string.IsNullOrWhiteSpace(user.TenTaiKhoan) || string.IsNullOrWhiteSpace(user.MatKhau))
        //     {
        //         TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ thông tin.";
        //         return View();
        //     }

        //     // Lấy thông tin tài khoản từ cơ sở dữ liệu
        //     var loggedInUser = _context.NguoiDungs
        //         .FirstOrDefault(u => u.TenTaiKhoan == user.TenTaiKhoan);

        //     if (loggedInUser == null)
        //     {
        //         TempData["ErrorMessage"] = "Tên tài khoản không tồn tại.";
        //         return View();
        //     }

        //     // Xác minh mật khẩu với BCrypt
        //     if (!BCrypt.Net.BCrypt.Verify(user.MatKhau, loggedInUser.MatKhau))
        //     {
        //         TempData["ErrorMessage"] = "Mật khẩu không đúng.";
        //         return View();
        //     }

        //     // Kiểm tra trạng thái tài khoản
        //     if (loggedInUser.TrangThai == false || loggedInUser.TrangThai == null)
        //     {
        //         TempData["ErrorMessage"] = "Tài khoản của bạn đã bị khóa.";
        //         return View();
        //     }

        //     // Gán giá trị session hoặc biến toàn cục
        //     Functions._MaNguoiDung = loggedInUser.MaNguoiDung;
        //     Functions._TenTaiKhoan = loggedInUser.TenTaiKhoan ?? string.Empty;
        //     Functions._Email = loggedInUser.Email ?? string.Empty;
        //     Functions._SoDienThoai = loggedInUser.SoDienThoai ?? string.Empty;

        //     HttpContext.Session.SetInt32("user", loggedInUser.MaNguoiDung);

        //     // Đăng nhập thành công, chuyển hướng về trang chính
        //     return RedirectToAction("Index", "Home", new { Area = "Admin" });
        // }
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public IActionResult Index(mentor.Models.tblNguoiDung user)
        // {
        //     if (user == null || string.IsNullOrWhiteSpace(user.TenTaiKhoan) || string.IsNullOrWhiteSpace(user.MatKhau))
        //     {
        //         TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ thông tin.";
        //         return View();
        //     }

        //     var loggedInUser = _context.NguoiDungs
        //         .FirstOrDefault(u => u.TenTaiKhoan == user.TenTaiKhoan);

        //     if (loggedInUser == null)
        //     {
        //         TempData["ErrorMessage"] = "Tên tài khoản không tồn tại.";
        //         return View();
        //     }

        //     if (!BCrypt.Net.BCrypt.Verify(user.MatKhau, loggedInUser.MatKhau))
        //     {
        //         TempData["ErrorMessage"] = "Mật khẩu không đúng.";
        //         return View();
        //     }

        //     if (loggedInUser.TrangThai == false || loggedInUser.TrangThai == null)
        //     {
        //         TempData["ErrorMessage"] = "Tài khoản của bạn đã bị khóa.";
        //         return View();
        //     }

        //     // Lưu thông tin vào Session
        //     HttpContext.Session.SetInt32("user", loggedInUser.MaNguoiDung);
        //     HttpContext.Session.SetString("role", loggedInUser.VaiTro!);

        //     // Điều hướng tới trang chủ sau khi đăng nhập thành công
        //     return RedirectToAction("Index", "Home", new { Area = "Admin" });
        // }

        // Phương thức POST để xử lý đăng nhập
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(mentor.Models.tblNguoiDung user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.TenTaiKhoan) || string.IsNullOrWhiteSpace(user.MatKhau))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            var loggedInUser = _context.NguoiDungs
                .FirstOrDefault(u => u.TenTaiKhoan == user.TenTaiKhoan);

            if (loggedInUser == null)
            {
                TempData["ErrorMessage"] = "Tên tài khoản không tồn tại.";
                return View();
            }

            if (!BCrypt.Net.BCrypt.Verify(user.MatKhau, loggedInUser.MatKhau))
            {
                TempData["ErrorMessage"] = "Mật khẩu không đúng.";
                return View();
            }

            if (loggedInUser.TrangThai == false || loggedInUser.TrangThai == null)
            {
                TempData["ErrorMessage"] = "Tài khoản của bạn đã bị khóa.";
                return View();
            }

            // Lưu thông tin vào Session
            HttpContext.Session.SetInt32("user", loggedInUser.MaNguoiDung);
            HttpContext.Session.SetString("role", loggedInUser.VaiTro!);

            // Thiết lập quyền truy cập theo vai trò
            SetUserPermissions(loggedInUser.VaiTro!);

            // Điều hướng tới trang chủ sau khi đăng nhập thành công
            return RedirectToAction("Index", "Home", new { Area = "Admin" });
        }

        // Thiết lập quyền truy cập dựa trên vai trò
        private void SetUserPermissions(string role)
        {
            if (role == "GiangVien")
            {
                HttpContext.Session.SetString("CanAccessDiemDanh", "True");
                HttpContext.Session.SetString("CanAccessThongKe", "False");
                HttpContext.Session.SetString("CanAccessNguoiDung", "False");
                HttpContext.Session.SetString("CanAccessHocPhan", "False");
            }
            else if (role == "Admin")
            {
                HttpContext.Session.SetString("CanAccessDiemDanh", "True");
                HttpContext.Session.SetString("CanAccessThongKe", "True");
                HttpContext.Session.SetString("CanAccessNguoiDung", "True");
                HttpContext.Session.SetString("CanAccessHocPhan", "True");
            }
            else if (role == "Manager")
            {
                HttpContext.Session.SetString("CanAccessDiemDanh", "False");
                HttpContext.Session.SetString("CanAccessThongKe", "True");
                HttpContext.Session.SetString("CanAccessNguoiDung", "False");
                HttpContext.Session.SetString("CanAccessHocPhan", "False");
            }
            else
            {
                // Mặc định không có quyền
                HttpContext.Session.SetString("CanAccessDiemDanh", "False");
                HttpContext.Session.SetString("CanAccessThongKe", "False");
                HttpContext.Session.SetString("CanAccessNguoiDung", "False");
                HttpContext.Session.SetString("CanAccessHocPhan", "False");
            }
        }
        
    }
}
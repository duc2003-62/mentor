// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using mentor.Areas.Admin.Models;
// using mentor.Models;
// using mentor.Utilities;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.Rendering;
// using Microsoft.EntityFrameworkCore;

// namespace mentor.Areas.Admin.Controllers
// {
//     [Area("Admin")]
//     public class DangKyController : Controller
//     {
//         private readonly DataContext _context;
//         public DangKyController(DataContext context)
//         {
//             _context = context;
//         }

//         public IActionResult Index()
//         {
//             return View();
//         }

//         [HttpPost]
//         public ActionResult Index(mentor.Models.tblNguoiDung auser)
//         {
//             // Kiểm tra đầu vào hợp lệ
//             if (auser == null || string.IsNullOrWhiteSpace(auser.TenTaiKhoan) || string.IsNullOrWhiteSpace(auser.MatKhau))
//             {
//                 ModelState.AddModelError("", "Vui lòng điền đầy đủ thông tin.");
//                 return View(auser);
//             }

//             // Kiểm tra tài khoản đã tồn tại
//             var check = _context.NguoiDungs.Any(u => u.TenTaiKhoan == auser.TenTaiKhoan);
//             if (check)
//             {
//                 TempData["ErrorMessage"] = "Tên người dùng đã tồn tại!";
//                 return RedirectToAction("Index", "DangKy");
//             }

//             // Gán giá trị mặc định
//             auser.VaiTro = "Sinh viên"; // Ngầm định vai trò là Sinh viên
//             auser.NgayTao = DateTime.Now; // Ngầm định ngày tạo là ngày hiện tại

//             // Mã hóa mật khẩu
//             auser.MatKhau = Functions.MD5Password(auser.MatKhau);

//             // Lưu người dùng vào cơ sở dữ liệu
//             _context.NguoiDungs.Add(auser);
//             _context.SaveChanges();

//             // Chuyển hướng với thông báo thành công
//             TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
//             return RedirectToAction("Index", "DangNhap");
//         }
//     }
// }
using System;
using System.Linq;
using mentor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace mentor.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DangKyController : Controller
    {
        private readonly DataContext _context;
        
        public DangKyController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(mentor.Models.tblNguoiDung auser)
        {
            // Kiểm tra đầu vào hợp lệ
            if (auser == null || string.IsNullOrWhiteSpace(auser.TenTaiKhoan) || string.IsNullOrWhiteSpace(auser.MatKhau))
            {
                ModelState.AddModelError("", "Vui lòng điền đầy đủ thông tin.");
                return View(auser);
            }

            // Kiểm tra tài khoản đã tồn tại
            var check = _context.NguoiDungs.Any(u => u.TenTaiKhoan == auser.TenTaiKhoan);
            if (check)
            {
                ModelState.AddModelError("", "Tên người dùng đã tồn tại!");
                return View(auser);
            }

            // Gán giá trị mặc định
            auser.VaiTro = "Sinh viên"; // Ngầm định vai trò là Sinh viên
            auser.NgayTao = DateTime.Now; // Ngầm định ngày tạo là ngày hiện tại

            // Mã hóa mật khẩu với BCrypt thay vì MD5
            auser.MatKhau = BCrypt.Net.BCrypt.HashPassword(auser.MatKhau);

            // Lưu người dùng vào cơ sở dữ liệu
            _context.NguoiDungs.Add(auser);
            _context.SaveChanges();

            // Chuyển hướng với thông báo thành công
            TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToAction("Index", "DangNhap");
        }
    }
}

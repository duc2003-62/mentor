using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mentor.Areas.Admin.Models;
using mentor.Models;
using mentor.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace mentor.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoginController : Controller
    {
        private readonly DataContext _context;
        public LoginController (DataContext context)
        {
            _context = context;
        }
        // [HttpPost]
        // public IActionResult Index(mentor.Models.tblNguoiDung user)
        // {
        //     if(user == null)
        //         return NotFound();
        //     string pw = Functions.MD5Password(user.MatKhau);
        //     var check = _context.NguoiDungs.Where(u => (u.TenTaiKhoan == user.TenTaiKhoan) && (u.MatKhau == pw)).FirstOrDefault();
        //     if(check == null)
        //     {
        //         Functions._Message = "InvaLid username and password!";
        //         return RedirectToAction("Index","Login");
        //     }
        //     Functions._Message = string.Empty;
        //     Functions._MaNguoiDung = check.MaNguoiDung;
        //     Functions._TenTaiKhoan = string.IsNullOrEmpty(check.TenTaiKhoan) ? string.Empty : check.TenTaiKhoan;
        //     Functions._Email = string.IsNullOrEmpty(check.Email) ? string.Empty : check.Email;
        //     return RedirectToAction("Index","Home");
        // }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(mentor.Models.tblNguoiDung user)
        {
            if (user == null)
                return NotFound();
                
            string pw = Functions.MD5Password(user.MatKhau);
            var check = _context.NguoiDungs
                .Where(u => u.TenTaiKhoan == user.TenTaiKhoan && u.MatKhau == pw)
                .FirstOrDefault();
            
            if (check == null)
            {
                Functions._Message = "Invalid username and password!";
                return RedirectToAction("Index", "DangNhap");
            }

            Functions._Message = string.Empty;
            Functions._MaNguoiDung = check.MaNguoiDung;
            Functions._TenTaiKhoan = check.TenTaiKhoan ?? string.Empty;
            Functions._Email = check.Email ?? string.Empty;
            Functions._SoDienThoai = check.SoDienThoai ?? string.Empty;
            return RedirectToAction("Index", "Home");
        }
    }
}
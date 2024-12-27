using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using mentor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace mentor.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class NguoiDungController : Controller
    {
        private readonly DataContext _context;
        public NguoiDungController (DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var mnList = _context.NguoiDungs.OrderBy(m => m.MaNguoiDung).ToList();
            return View(mnList);
        }
        public IActionResult Create()
        {
            var rolesList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Quản trị viên", Value = "Quản trị viên" },
                new SelectListItem { Text = "Giảng viên", Value = "Giảng viên" },
                new SelectListItem { Text = "Học sinh", Value = "Học sinh" }
            };

            ViewBag.RolesList = rolesList;

            // Trạng thái mặc định cho trạng thái người dùng
            ViewBag.StatusList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Hoạt động", Value = "true", Selected = true },
                new SelectListItem { Text = "Không hoạt động", Value = "false" }
            };

            return View();
        }
        // [HttpPost]
        // public IActionResult Create(tblNguoiDung user)
        // {
        //     // Kiểm tra trùng lặp Tên Tài Khoản
        //     if (_context.NguoiDungs.Any(u => u.TenTaiKhoan == user.TenTaiKhoan))
        //     {
        //         ModelState.AddModelError("TenTaiKhoan", "Tên tài khoản đã tồn tại.");
        //     }

        //     // Kiểm tra trùng lặp Email
        //     if (_context.NguoiDungs.Any(u => u.Email == user.Email))
        //     {
        //         ModelState.AddModelError("Email", "Email đã tồn tại.");
        //     }

        //     // Kiểm tra nếu mật khẩu quá ngắn (nếu cần)
        //     if (user.MatKhau?.Length < 6)
        //     {
        //         ModelState.AddModelError("MatKhau", "Mật khẩu phải có ít nhất 6 ký tự.");
        //         // Tạo lại danh sách vai trò để hiển thị đúng dropdown
        //         ViewBag.DanhSachVaiTro = new List<string> { "Quản trị viên", "Giảng viên", "Sinh viên" };
        //         return View(user);
        //     }
        //     // Đặt giá trị mặc định nếu trạng thái không được chọn
        //     if (user.TrangThai == null)
        //     {
        //         user.TrangThai = true; // Hoặc false tùy mặc định
        //     }

        //     // Kiểm tra tính hợp lệ của ModelState
        //     if (ModelState.IsValid)
        //     {
        //         try
        //         {
        //             // Đặt ngày tạo mặc định nếu chưa có
        //             user.NgayTao = DateTime.Now.Date;

        //             // Mã hóa mật khẩu trước khi lưu vào cơ sở dữ liệu
        //             user.MatKhau = BCrypt.Net.BCrypt.HashPassword(user.MatKhau); // Sử dụng thư viện BCrypt để mã hóa mật khẩu

        //             // Đặt trạng thái mặc định nếu không được chọn
        //             if (user.TrangThai == null)
        //             {
        //                 user.TrangThai = true; // Set default to "Hoạt động"
        //             }

        //             // Thêm người dùng mới vào cơ sở dữ liệu
        //             _context.NguoiDungs.Add(user);
        //             _context.SaveChanges();

        //             // Chuyển hướng về trang Index sau khi tạo thành công
        //             TempData["SuccessMessage"] = "Người dùng đã được tạo thành công.";
        //             return RedirectToAction("Index");
        //         }
        //         catch (Exception ex)
        //         {
        //             // Log error
        //             Debug.WriteLine($"Error occurred while creating user: {ex.Message}");
        //             ModelState.AddModelError("", "Có lỗi xảy ra khi tạo người dùng.");
        //         }
        //     }

        //     // Nếu có lỗi, trả lại trang Create và hiển thị lỗi
        //     return View(user);
        // }
        [HttpPost]
        public IActionResult Create(tblNguoiDung user)
        {
            // Kiểm tra trùng lặp Tên Tài Khoản
            if (_context.NguoiDungs.Any(u => u.TenTaiKhoan == user.TenTaiKhoan))
            {
                ModelState.AddModelError("TenTaiKhoan", "Tên tài khoản đã tồn tại.");
            }

            // Kiểm tra trùng lặp Email
            if (_context.NguoiDungs.Any(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Email đã tồn tại.");
            }

            // Kiểm tra nếu mật khẩu quá ngắn
            if (user.MatKhau?.Length < 6)
            {
                ModelState.AddModelError("MatKhau", "Mật khẩu phải có ít nhất 6 ký tự.");
            }

            // Đặt giá trị mặc định nếu trạng thái không được chọn
            if (user.TrangThai == null)
            {
                user.TrangThai = true; // Hoặc false tùy mặc định
            }

            // Kiểm tra tính hợp lệ của ModelState
            if (ModelState.IsValid)
            {
                try
                {
                    // Đặt ngày tạo mặc định nếu chưa có
                    user.NgayTao = DateTime.Now.Date;

                    // Mã hóa mật khẩu trước khi lưu vào cơ sở dữ liệu
                    user.MatKhau = BCrypt.Net.BCrypt.HashPassword(user.MatKhau); // Mã hóa mật khẩu

                    // Thêm người dùng mới vào cơ sở dữ liệu
                    _context.NguoiDungs.Add(user);
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "Người dùng đã được tạo thành công.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error occurred while creating user: {ex.Message}");
                    ModelState.AddModelError("", "Có lỗi xảy ra khi tạo người dùng.");
                }
            }

            // Nếu có lỗi, tái khởi tạo danh sách Vai trò và Trạng thái
            ViewBag.RolesList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Quản trị viên", Value = "Quản trị viên" },
                new SelectListItem { Text = "Giảng viên", Value = "Giảng viên" },
                new SelectListItem { Text = "Học sinh", Value = "Học sinh" }
            };

            ViewBag.StatusList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Hoạt động", Value = "true", Selected = true },
                new SelectListItem { Text = "Không hoạt động", Value = "false" }
            };

            return View(user); // Trả lại View với dữ liệu người dùng đã nhập
        }

        public IActionResult Delete(int? id)
        {
            // Kiểm tra nếu ID không hợp lệ
            if (id == null || id == 0)
                return NotFound();

            // Tìm thông tin người dùng theo ID
            var user = _context.NguoiDungs.Find(id);
            if (user == null)
                return NotFound();

            return View(user); // Hiển thị trang xác nhận xóa
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            // Tìm người dùng cần xóa
            var userToDelete = _context.NguoiDungs.Find(id);
            if (userToDelete == null)
                return NotFound();
            // Xóa người dùng
            _context.NguoiDungs.Remove(userToDelete);
            _context.SaveChanges();
            return RedirectToAction("Index"); // Quay lại danh sách người dùng
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            // Tìm người dùng cần chỉnh sửa
            var user = _context.NguoiDungs.Find(id);
            if (user == null)
                return NotFound();

            // Tạo danh sách Vai trò và đánh dấu vai trò hiện tại
            var roleList = new List<SelectListItem>
            {
                new SelectListItem { Value = "Quản trị viên", Text = "Quản trị viên", Selected = user.VaiTro == "Quản trị viên" },
                new SelectListItem { Value = "Giảng viên", Text = "Giảng viên", Selected = user.VaiTro == "Giảng viên" },
                new SelectListItem { Value = "Sinh viên", Text = "Sinh viên", Selected = user.VaiTro == "Sinh viên" },
            };

            ViewBag.RolesList = roleList; // Truyền danh sách vào View
            return View(user); // Truyền thông tin người dùng vào View
        }
        [HttpPost]
        public IActionResult Edit(tblNguoiDung user)
        {
            // Lấy thông tin người dùng hiện tại từ cơ sở dữ liệu
            var existingUser = _context.NguoiDungs.Find(user.MaNguoiDung);
            if (existingUser == null)
            {
                return NotFound();
            }

            // Kiểm tra email trùng lặp (ngoại trừ người đang chỉnh sửa)
            if (user.Email != existingUser.Email && // Nếu email thay đổi
                _context.NguoiDungs.Any(u => u.Email == user.Email && u.MaNguoiDung != user.MaNguoiDung))
            {
                ModelState.AddModelError("Email", "Email đã tồn tại.");
            }

            // Kiểm tra tên tài khoản trùng lặp (ngoại trừ người đang chỉnh sửa)
            if (user.TenTaiKhoan != existingUser.TenTaiKhoan && // Nếu tên tài khoản thay đổi
                _context.NguoiDungs.Any(u => u.TenTaiKhoan == user.TenTaiKhoan && u.MaNguoiDung != user.MaNguoiDung))
            {
                ModelState.AddModelError("TenTaiKhoan", "Tên tài khoản đã tồn tại.");
            }
            // Nếu không có lỗi thì tiếp tục xử lý
            if (ModelState.IsValid)
            {
                // Cập nhật thông tin người dùng
                existingUser.TenTaiKhoan = user.TenTaiKhoan;
                existingUser.Email = user.Email;
                existingUser.SoDienThoai = user.SoDienThoai;
                if (!string.IsNullOrWhiteSpace(user.MatKhau))
                {
                    existingUser.MatKhau = BCrypt.Net.BCrypt.HashPassword(user.MatKhau);
                }
                existingUser.VaiTro = user.VaiTro; // Cập nhật vai trò mới
                existingUser.TrangThai = user.TrangThai;

                // Lưu thay đổi vào cơ sở dữ liệu
                _context.NguoiDungs.Update(existingUser);
                _context.SaveChanges();

                // Quay lại danh sách người dùng
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, trả lại danh sách Vai trò và hiển thị lại form
            var roleList = new List<SelectListItem>
            {
                new SelectListItem { Value = "Quản trị viên", Text = "Quản trị viên" },
                new SelectListItem { Value = "Giảng viên", Text = "Giảng viên" },
                new SelectListItem { Value = "Sinh viên", Text = "Sinh viên" }
            };
            ViewBag.RoleList = roleList;

            return View(user);
        }
        [HttpGet]
        public IActionResult ToggleStatus(int id)
        {
            // Tìm người dùng theo ID
            var nguoiDung = _context.NguoiDungs.Find(id);

            if (nguoiDung == null)
            {
                return NotFound(); // Nếu không tìm thấy người dùng, trả về lỗi 404
            }

            // Đảo trạng thái: Hoạt động (true) -> Không hoạt động (false) và ngược lại
            nguoiDung.TrangThai = !nguoiDung.TrangThai;

            // Lưu thay đổi vào cơ sở dữ liệu
            _context.SaveChanges();

            // Trả về trang Index sau khi thay đổi
            return RedirectToAction("Index");
        }


    }
}
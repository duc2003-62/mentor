using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using mentor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace mentor.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LopBienCheController : Controller
    {
        private readonly DataContext _context;
        public LopBienCheController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var mnList = _context.LopBienChes
            .Include(lb => lb.Khoa) // Load dữ liệu liên kết
            .OrderBy(m => m.MaLop)
            .ToList();
        return View(mnList);
        }
        public IActionResult Delete(long? id)
        {
            if (id == null || id == 0)
                return NotFound();
            var mn= _context.LopBienChes.Find(id);
            if (mn == null)
                return NotFound();
            return View(mn);
        }
        [HttpPost]
        public IActionResult Delete(long id)
        {
            var delLopBienChe = _context.LopBienChes.Find(id);
            if (delLopBienChe == null)
                return NotFound();

            // Kiểm tra xem có sinh viên nào liên kết với lớp biên chế này không
            bool hasActiveSinhVien = _context.SinhViens.Any(sv => sv.MaLop == id);
            if (hasActiveSinhVien)
            {
                // Thêm thông báo lỗi vào ModelState
                ModelState.AddModelError("", "Không thể xóa lớp biên chế này vì có sinh viên đang hoạt động trong lớp.");
                return View("Index", _context.LopBienChes.ToList()); // Quay về trang Index và hiển thị thông báo lỗi
            }

            _context.LopBienChes.Remove(delLopBienChe);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Create()
        {
            // Lấy danh sách các khoa để hiển thị trong dropdown
            var khoaList = (from k in _context.Khoas
                            select new SelectListItem()
                            {
                                Text = k.TenKhoa,
                                Value = k.MaKhoa.ToString()
                            }).ToList();

            // Thêm tùy chọn mặc định
            khoaList.Insert(0, new SelectListItem()
            {
                Text = "--- select ---",
                Value = "0"
            });

            // Gửi danh sách đến View thông qua ViewBag
            ViewBag.KhoaList = khoaList;

            return View();
        }

        [HttpPost]
        public IActionResult Create(tblLopBienChe lopBienChe)
        {
            if (_context.LopBienChes.Any(k => k.TenLop== lopBienChe.TenLop))
            {
                ModelState.AddModelError("TenLop", "Tên Lớp Biên Chế đã tồn tại.");
            }
            if (ModelState.IsValid)
            {
                // Thêm lớp biên chế mới vào CSDL
                _context.LopBienChes.Add(lopBienChe);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, gửi lại danh sách khoa để hiển thị lại dropdown
            ViewBag.KhoaList = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa", lopBienChe.MaKhoa);
            return View(lopBienChe);
        }
        public IActionResult Edit(long? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var lopBienChe = _context.LopBienChes.Find(id);
            if (lopBienChe == null)
                return NotFound();

            // Get the list of Khoas to populate the dropdown
            var mnList = (from m in _context.Khoas
                        select new SelectListItem()
                        {
                            Value = m.MaKhoa.ToString(),
                            Text = m.TenKhoa
                        }).ToList();
            mnList.Insert(0, new SelectListItem()
            {
                Text = "--- select ---",
                Value = "0"
            });
            ViewBag.mnList = mnList;

            return View(lopBienChe);
        }

        [HttpPost]
        public IActionResult Edit(tblLopBienChe lopBienChe)
        {
            if (ModelState.IsValid)
            {
                // Load the existing record to keep the original values
                var existingRecord = _context.LopBienChes.Find(lopBienChe.MaLop);
                if (existingRecord == null)
                {
                    return NotFound();
                }

                // Update the fields
                existingRecord.TenLop = lopBienChe.TenLop;
                existingRecord.MaKhoa = lopBienChe.MaKhoa;
                existingRecord.NamHoc = lopBienChe.NamHoc;

                // Save the changes
                _context.LopBienChes.Update(existingRecord);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            // If the model state is invalid, return the view with the entered data
            return View(lopBienChe);
        }

    }
}
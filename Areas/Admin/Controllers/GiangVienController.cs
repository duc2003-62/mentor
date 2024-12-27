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
    public class GiangVienController : Controller
    {
        private readonly DataContext _context;
        public GiangVienController(DataContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var mnList = await _context.GiangViens
                                       .Include(s => s.Khoa)        // Lấy thông tin Khoa liên quan
                                       .OrderBy(m => m.MaGiangVien)
                                       .ToListAsync();

            return View(mnList);
        }
        public IActionResult Delete(long? id)
        {
            if (id == null || id == 0)
                return NotFound();
            var mn= _context.GiangViens.Find(id);
            if (mn == null)
                return NotFound();
            return View(mn);
        }
        [HttpPost]
        public IActionResult Delete(long id)
        {
            var delGiangVien= _context.GiangViens.Find(id);
            if (delGiangVien == null)
                return NotFound();
             bool hasActiveLopHocPhan = _context.LopHocPhans.Any(lbc => lbc.MaGiangVien == id);
             if (hasActiveLopHocPhan)
            {
                // Thêm thông báo lỗi vào ModelState
                ModelState.AddModelError("", "Không thể xóa giảng viên này vì giảng viên này đang có lớp học phần.");
                return View("Index", _context.GiangViens.ToList()); // Quay về trang Index và hiển thị thông báo lỗi
            }
            _context.GiangViens.Remove(delGiangVien);
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

            // Thêm tùy chọn mặc định cho dropdown Khoa
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
        public IActionResult Create(tblGiangVien giangVien)
        {
            if (ModelState.IsValid)
            {
                // Thêm sinh viên mới vào CSDL
                _context.GiangViens.Add(giangVien);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, gửi lại danh sách khoa để hiển thị lại dropdown
            ViewBag.KhoaList = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa", giangVien.MaKhoa);   
            return View(giangVien);
        }
        public IActionResult Edit(long? id)
        {
            if (id == null || id == 0)
                return NotFound();

            // Tìm sinh viên theo id
            var giangvien = _context.GiangViens.Find(id);
            if (giangvien == null)
                return NotFound();

            // Lấy danh sách Khoa để hiển thị trong dropdown
            var khoaList = (from k in _context.Khoas
                            select new SelectListItem()
                            {
                                Value = k.MaKhoa.ToString(),
                                Text = k.TenKhoa
                            }).ToList();
            khoaList.Insert(0, new SelectListItem()
            {
                Text = "--- select ---",
                Value = "0"
            });
            ViewBag.KhoaList = khoaList;
            return View(giangvien);
        }

        [HttpPost]
        public IActionResult Edit(tblGiangVien giangvien)
        {
            if (ModelState.IsValid)
            {
                // Tải bản ghi hiện tại để giữ nguyên các giá trị gốc
                var existingRecord = _context.GiangViens.Find(giangvien.MaGiangVien);
                if (existingRecord == null)
                {
                    return NotFound();
                }

                // Cập nhật các trường
                existingRecord.HoTenGiangVien = giangvien.HoTenGiangVien;
                existingRecord.NgaySinh = giangvien.NgaySinh;
                existingRecord.GioiTinh = giangvien.GioiTinh;
                existingRecord.MaKhoa = giangvien.MaKhoa;
                existingRecord.Email = giangvien.Email;
                existingRecord.SoDienThoai = giangvien.SoDienThoai;
                existingRecord.DiaChi = giangvien.DiaChi;

                // Lưu các thay đổi
                _context.GiangViens.Update(existingRecord);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            // Nếu trạng thái mô hình không hợp lệ, trả lại view với dữ liệu đã nhập
            ViewBag.KhoaList = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa", giangvien.MaKhoa);
            return View(giangvien);
        }
    }
}
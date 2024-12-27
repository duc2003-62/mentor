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
    public class HocPhanController : Controller
    {
        private readonly DataContext _context;
        public HocPhanController(DataContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var mnList = await _context.HocPhans
                                       .Include(s => s.Khoa)        // Lấy thông tin Khoa liên quan
                                       .OrderBy(m => m.MaHocPhan)
                                       .ToListAsync();

            return View(mnList);
        }
        public IActionResult Delete(long? id)
        {
            if (id == null || id == 0)
                return NotFound();
            var mn= _context.HocPhans.Find(id);
            if (mn == null)
                return NotFound();
            return View(mn);
        }
        [HttpPost]
        public IActionResult Delete(long id)
        {
            var delHocPhan= _context.HocPhans.Find(id);
            if (delHocPhan == null)
                return NotFound();
            _context.HocPhans.Remove(delHocPhan);
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
            ViewBag.SoTinChiList = new SelectList(Enumerable.Range(1, 10).Select(x => new { Value = x, Text = x }), "Value", "Text");
            return View();
        }

        [HttpPost]
        public IActionResult Create(tblHocPhan hocPhan)
        {
            if (hocPhan.MaKhoa == 0)
            {
                ModelState.AddModelError("MaKhoa", "Vui lòng chọn một khoa hợp lệ.");
            }
            // Kiểm tra trùng lặp Tên học phần
            if (_context.HocPhans.Any(k => k.TenHocPhan == hocPhan.TenHocPhan))
            {
                ModelState.AddModelError("TenHocPhan", "Tên học phần đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                // Thêm học phần mới vào CSDL
                _context.HocPhans.Add(hocPhan);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, gửi lại danh sách khoa để hiển thị lại dropdown
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
            ViewBag.KhoaList = khoaList;

            // Đặt lại danh sách số tín chỉ
            ViewBag.SoTinChiList = new SelectList(Enumerable.Range(1, 10).Select(x => new { Value = x, Text = x }), "Value", "Text");

            return View(hocPhan);
        }
        public IActionResult Edit(long? id)
        {
            if (id == null || id == 0)
                return NotFound();

            // Tìm học phần theo id
            var HocPhan = _context.HocPhans.Find(id);
            if (HocPhan == null)
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
             ViewBag.SoTinChiList = new SelectList(Enumerable.Range(1, 10).Select(x => new { Value = x, Text = x }), "Value", "Text");
            return View(HocPhan);
        }

        [HttpPost]
        public IActionResult Edit(tblHocPhan HocPhan)
        {
            if (ModelState.IsValid)
            {
                // Tải bản ghi hiện tại để giữ nguyên các giá trị gốc
                var existingRecord = _context.HocPhans.Find(HocPhan.MaHocPhan);
                if (existingRecord == null)
                {
                    return NotFound();
                }

                // Cập nhật các trường
                existingRecord.TenHocPhan = HocPhan.TenHocPhan;
                existingRecord.SoTinChi= HocPhan.SoTinChi;
                existingRecord.MaKhoa = HocPhan.MaKhoa;

                // Lưu các thay đổi
                _context.HocPhans.Update(existingRecord);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            // Nếu trạng thái mô hình không hợp lệ, trả lại view với dữ liệu đã nhập
            ViewBag.KhoaList = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa", HocPhan.MaKhoa);
            return View(HocPhan);
        }
    }
}
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
    public class LopHocPhanController : Controller
    {
        private readonly DataContext _context;
        public LopHocPhanController(DataContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index() 
        {
            var lopHocPhanList = await _context.LopHocPhans
                                                .Include(lhp => lhp.HocPhan)        // Lấy thông tin Học phần liên quan
                                                .Include(lhp => lhp.GiangVien)      // Lấy thông tin Giảng viên liên quan
                                                .OrderBy(lhp => lhp.MaLopHocPhan)
                                                .ToListAsync();
            if (lopHocPhanList == null)
            {
                return NotFound();
            }
            return View(lopHocPhanList);
        }
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lopHocPhan1 = _context.LopHocPhans
                .Include(l => l.GiangVien) // Bao gồm GiangVien
                .Include(l => l.HocPhan)   // Bao gồm HocPhan
                .FirstOrDefault(m => m.MaLopHocPhan == id);

            if (lopHocPhan1 == null)
            {
                return NotFound();
            }

            return View(lopHocPhan1);
        }
        public IActionResult Create()
        {
            // Lấy danh sách các giảng viên để hiển thị trong dropdown
            var giangVienList = (from gv in _context.GiangViens
                                select new SelectListItem()
                                {
                                    Text = gv.HoTenGiangVien,
                                    Value = gv.MaGiangVien.ToString()
                                }).ToList();

            // Thêm tùy chọn mặc định cho dropdown Giảng Viên
            giangVienList.Insert(0, new SelectListItem()
            {
                Text = "--- select ---",
                Value = "0"
            });

            // Gửi danh sách đến View thông qua ViewBag
            ViewBag.HocPhanList = new SelectList(_context.HocPhans, "MaHocPhan", "TenHocPhan");
            ViewBag.GiangVienList = new SelectList(_context.GiangViens, "MaGiangVien", "HoTenGiangVien");
            // If you have a KhoaList, set it up similarly
            ViewBag.KhoaList = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa"); 
            return View();
        }

        [HttpPost]
        public IActionResult Create(tblLopHocPhan lopHocPhan)
        {
            // Kiểm tra trùng lặp Tên lớp học phần
            if (_context.LopHocPhans.Any(k => k.TenLopHocPhan == lopHocPhan.TenLopHocPhan))
            {
                ModelState.AddModelError("TenLopHocPhan", "Tên lớp học phần đã tồn tại.");
            }
            if (ModelState.IsValid)
            {
                _context.LopHocPhans.Add(lopHocPhan);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            // Re-fetch lists if model validation fails
            ViewBag.GiangVienList = _context.GiangViens
                .Select(g => new SelectListItem { Text = g.HoTenGiangVien, Value = g.MaGiangVien.ToString() }).ToList();
            ViewBag.HocPhanList = _context.HocPhans
                .Select(h => new SelectListItem { Text = h.TenHocPhan, Value = h.MaHocPhan.ToString() }).ToList();
            ViewBag.SoTinChiList = new SelectList(Enumerable.Range(1, 10).Select(x => new { Value = x, Text = x }), "Value", "Text");

            return View(lopHocPhan);
        }
        public IActionResult Delete(long? id)
        {
            if (id == null || id == 0)
                return NotFound();
            var lopHocPhan = _context.LopHocPhans.Find(id); // Tìm LopHocPhan theo id
            if (lopHocPhan == null)
                return NotFound();
            return View(lopHocPhan); // Trả về View cho LopHocPhan
        }

        [HttpPost]
        public IActionResult Delete(long id)
        {
            var delLopHocPhan = _context.LopHocPhans.Find(id);
            if (delLopHocPhan == null)
                return NotFound();

            // Kiểm tra xem có DiemDanh nào liên kết với LopHocPhan này không
            //bool hasActiveDiemDanh = _context.DiemDanhs.Any(dd => dd.MaLopHocPhan == id);
            
            // Kiểm tra xem có SinhVien nào liên kết với LopHocPhan này không
            bool hasActiveSinhVien = _context.SinhViens.Any(sv => sv.MaSinhVien == id);
            
            // Kiểm tra xem có GiangVien nào liên kết với LopHocPhan này không
            bool hasActiveGiangVien = _context.GiangViens.Any(gv => gv.MaGiangVien == id);

            bool hasActiveHocPhan = _context.HocPhans.Any(gv => gv.MaHocPhan == id);

            if ( hasActiveSinhVien || hasActiveGiangVien || hasActiveHocPhan)
            {
                // Thêm thông báo lỗi vào ModelState
                ModelState.AddModelError("", "Không thể xóa lớp học phần này vì sinh viên hoặc giảng viên đang hoạt động trong lớp học phần này.");
                return View("Index", _context.LopHocPhans.ToList()); // Quay về trang Index và hiển thị thông báo lỗi
            }

            _context.LopHocPhans.Remove(delLopHocPhan);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Edit(long? id)
        {
            if (id == null || id == 0)
                return NotFound();

            // Tìm lớp học phần theo id
            var lopHocPhan = _context.LopHocPhans.Find(id);
            if (lopHocPhan == null)
                return NotFound();

            // Lấy danh sách học phần để hiển thị trong dropdown
            var hocPhanList = _context.HocPhans.Select(h => new SelectListItem
            {
                Value = h.MaHocPhan.ToString(),
                Text = h.TenHocPhan
            }).ToList();
            hocPhanList.Insert(0, new SelectListItem
            {
                Text = "--- select ---",
                Value = "0"
            });
            ViewBag.HocPhanList = hocPhanList;

            // Lấy danh sách giảng viên để hiển thị trong dropdown
            var giangVienList = _context.GiangViens.Select(g => new SelectListItem
            {
                Value = g.MaGiangVien.ToString(),
                Text = g.HoTenGiangVien // Giả sử bạn có thuộc tính TenGiangVien trong tblGiangVien
            }).ToList();
            giangVienList.Insert(0, new SelectListItem
            {
                Text = "--- select ---",
                Value = "0"
            });
            ViewBag.GiangVienList = giangVienList;

            return View(lopHocPhan);
        }

        [HttpPost]
        public IActionResult Edit(tblLopHocPhan lopHocPhan)
        {
            if (ModelState.IsValid)
            {
                // Tải bản ghi hiện tại để giữ nguyên các giá trị gốc
                var existingRecord = _context.LopHocPhans.Find(lopHocPhan.MaLopHocPhan);
                if (existingRecord == null)
                {
                    return NotFound();
                }

                // Cập nhật các trường
                existingRecord.TenLopHocPhan = lopHocPhan.TenLopHocPhan;
                existingRecord.MaHocPhan = lopHocPhan.MaHocPhan;
                existingRecord.MaGiangVien = lopHocPhan.MaGiangVien;
                existingRecord.HocKy = lopHocPhan.HocKy;
                existingRecord.NgayBatDau = lopHocPhan.NgayBatDau;
                existingRecord.NgayKetThuc = lopHocPhan.NgayKetThuc;
                existingRecord.NamHoc = lopHocPhan.NamHoc;

                // Lưu các thay đổi
                _context.LopHocPhans.Update(existingRecord);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            // Nếu trạng thái mô hình không hợp lệ, trả lại view với dữ liệu đã nhập
            ViewBag.HocPhanList = new SelectList(_context.HocPhans, "MaHocPhan", "TenHocPhan", lopHocPhan.MaHocPhan);
            ViewBag.GiangVienList = new SelectList(_context.GiangViens, "MaGiangVien", "HoTenGiangVien", lopHocPhan.MaGiangVien);
            return View(lopHocPhan);
        }


    }
}
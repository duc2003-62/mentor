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
    public class SinhVienController : Controller
    {
        private readonly DataContext _context;
        public SinhVienController(DataContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var mnList = await _context.SinhViens
                                       .Include(s => s.Khoa)        // Lấy thông tin Khoa liên quan
                                       .Include(s => s.LopBienChe)  // Lấy thông tin LopBienChe liên quan
                                       .OrderBy(m => m.MaSinhVien)
                                       .ToListAsync();

            return View(mnList);
        }
        // GET: Delete student (Confirmation Page)
        public IActionResult Delete(long? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var student = _context.SinhViens.Find(id);
            if (student == null)
                return NotFound();

            return View(student); // Display a confirmation page for deleting the student
        }

        // POST: Delete student (Actual Deletion)
        [HttpPost]
        public IActionResult Delete(long id)
        {
            var studentToDelete = _context.SinhViens.Find(id);
            if (studentToDelete == null)
                return NotFound();

            // Check if the student is in any active class sections

            // If no active class sections, proceed with deletion
            _context.SinhViens.Remove(studentToDelete);
            _context.SaveChanges();

            return RedirectToAction("Index"); // Redirect to the list of students after successful deletion
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

            // Lấy danh sách các lớp biên chế để hiển thị trong dropdown
            var lopList = (from l in _context.LopBienChes
                        select new SelectListItem()
                        {
                            Text = l.TenLop,
                            Value = l.MaLop.ToString()
                        }).ToList();

            // Thêm tùy chọn mặc định cho dropdown Khoa
            khoaList.Insert(0, new SelectListItem()
            {
                Text = "--- select ---",
                Value = "0"
            });

            // Thêm tùy chọn mặc định cho dropdown Lớp
            lopList.Insert(0, new SelectListItem()
            {
                Text = "--- select ---",
                Value = "0"
            });

            // Gửi danh sách đến View thông qua ViewBag
            ViewBag.KhoaList = khoaList;
            ViewBag.LopList = lopList;

            return View();
        }

        [HttpPost]
        public IActionResult Create(tblSinhVien sinhVien)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the highest current MaSinhVienCode, or set a default if none exists
                var lastStudent = _context.SinhViens
                    .OrderByDescending(s => s.MaSinhVienCode)
                    .FirstOrDefault();

                // Set the initial code if there are no existing students
                if (lastStudent?.MaSinhVienCode != null)
                {
                    // Convert the last MaSinhVienCode to a long, increment by 1, and pad with leading zeros
                    long newCode = long.Parse(lastStudent.MaSinhVienCode) + 1;
                    sinhVien.MaSinhVienCode = newCode.ToString("000000000000000"); // Keeps the format as "215714021010002" etc.
                }
                else
                {
                    // If no previous MaSinhVienCode exists, set the default value
                    sinhVien.MaSinhVienCode = "215714021010001";
                }
                // Add the new student to the database
                _context.SinhViens.Add(sinhVien);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            // Reload Khoa and Lop lists if there are validation errors
            ViewBag.KhoaList = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa", sinhVien.MaKhoa);
            ViewBag.LopList = new SelectList(_context.LopBienChes, "MaLop", "TenLop", sinhVien.MaLop);
            
            return View(sinhVien);
        }
        public IActionResult Edit(long id)
        {
            var sinhVien = _context.SinhViens.Find(id);
            if (sinhVien == null)
            {
                return NotFound();
            }

            // Tải danh sách Khoa và Lớp để hiển thị trong dropdown
            ViewBag.KhoaList = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa", sinhVien.MaKhoa);
            ViewBag.LopList = new SelectList(_context.LopBienChes, "MaLop", "TenLop", sinhVien.MaLop);

            return View(sinhVien);
        }

        [HttpPost]
        public IActionResult Edit(tblSinhVien sinhVien)
        {
            if (ModelState.IsValid)
            {
                // Tải bản ghi hiện tại để giữ nguyên các giá trị gốc
                var existingRecord = _context.SinhViens.Find(sinhVien.MaSinhVien);
                if (existingRecord == null)
                {
                    return NotFound();
                }

                // Cập nhật các trường
                existingRecord.MaSinhVienCode=sinhVien.MaSinhVienCode;
                existingRecord.HoTen = sinhVien.HoTen;
                existingRecord.NgaySinh = sinhVien.NgaySinh;
                existingRecord.GioiTinh = sinhVien.GioiTinh;
                existingRecord.MaKhoa = sinhVien.MaKhoa;
                existingRecord.MaLop = sinhVien.MaLop;
                existingRecord.Email = sinhVien.Email;
                existingRecord.SoDienThoai = sinhVien.SoDienThoai;
                existingRecord.DiaChi = sinhVien.DiaChi;

                // Lưu các thay đổi
                _context.SinhViens.Update(existingRecord);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            // Nếu trạng thái mô hình không hợp lệ, trả lại view với dữ liệu đã nhập
            ViewBag.KhoaList = new SelectList(_context.Khoas, "MaKhoa", "TenKhoa", sinhVien.MaKhoa);
            ViewBag.LopList = new SelectList(_context.LopBienChes, "MaLop", "TenLop", sinhVien.MaLop);
            return View(sinhVien);
        }


        [Route("api/search/{studentCode}")]
        public IActionResult GetStudent(string studentCode) {
            Console.WriteLine(studentCode);
            var student = _context.SinhViens.Where(s => s.MaSinhVienCode == studentCode).First();
            Console.WriteLine(student);
            if (student == null) {
                return NotFound("Khong ton tai sinh vien voi ma code");
            }
            return Ok(student);
        }
    }
}
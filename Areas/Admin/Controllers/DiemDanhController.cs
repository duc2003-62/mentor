using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mentor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace mentor.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DiemDanhController : Controller
    {
        private static readonly Dictionary<string, string> STATUS = new Dictionary<string, string>
        {
            { "CM", "Có mặt" },
            { "M", "Muộn" },
            { "CP", "Có phép" },
            { "KP", "Không phép" },
        };

        private readonly DataContext _context;

        public DiemDanhController(DataContext context)
        {
            _context = context;
        }

        // Load the main attendance page
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách học kỳ để hiển thị trong Dropdown
            ViewBag.HocKyOptions = new SelectList(
                await _context.LopHocPhans.Select(l => l.HocKy).Distinct().ToListAsync()
            );

            // Lấy danh sách lớp học phần
            var lopHocPhans = await _context.LopHocPhans.ToListAsync();
            ViewBag.LopHocPhanOptions = new SelectList(lopHocPhans, "MaLopHocPhan", "TenLopHocPhan");

            ViewBag.BuoiList = new SelectList(
                Enumerable.Range(1, 20).Select(x => new { Value = x, Text = x }), "Value", "Text"
            );

            return View();
        }

        // Action to get students based on selected LopHocPhan (class section)
        [HttpPost]
        public async Task<IActionResult> GetStudents([FromBody] FormDataModel formData)
        {
            var students = await _context.SinhVienCuaLopHocPhans
                .Where(sv => sv.MaLopHocPhan == formData.MaLopHocPhan)
                .Include(sv => sv.SinhVien) // Bao gồm thông tin sinh viên
                .ToListAsync();

            // Lấy danh sách điểm danh cho buổi hiện tại
            var attendanceRecords = await _context.DiemDanhs
                .Where(dd => dd.Buoi == formData.Buoi && 
                             dd.NgayDiemDanh == formData.NgayDiemDanh &&
                             dd.MaSinhVienCuaLopHocPhan != null)
                .ToListAsync();

            var results = students.Select(sv => new Dictionary<string, object>
            {
                { "MaSinhVienCuaLopHocPhan", sv.MaSinhVienCuaLopHocPhan },
                { "MaSinhVien", sv.SinhVien!.MaSinhVien },
                { "MaSinhVienCode", sv.SinhVien.MaSinhVienCode! },
                { "HoTen", sv.SinhVien.HoTen! },
                { "TrangThai", attendanceRecords.FirstOrDefault(a => a.MaSinhVienCuaLopHocPhan == sv.MaSinhVienCuaLopHocPhan)?.TrangThai ?? " " },
                { "GhiChu", attendanceRecords.FirstOrDefault(a => a.MaSinhVienCuaLopHocPhan == sv.MaSinhVienCuaLopHocPhan)?.GhiChu ?? "" }
            }).ToList();

            return Json(results);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAttendance([FromBody] List<SaveDataModel> attendanceData)
        {
            if (attendanceData == null || !attendanceData.Any())
            {
                return Json(new { success = false, message = "Dữ liệu điểm danh không hợp lệ." });
            }

            try
            {
                foreach (var item in attendanceData)
                {
                    var existingRecord = await _context.DiemDanhs
                        .FirstOrDefaultAsync(dd => dd.MaSinhVienCuaLopHocPhan == item.MaSinhVienCuaLopHocPhan && 
                                                   dd.Buoi == item.Buoi && 
                                                   dd.NgayDiemDanh == item.NgayDiemDanh);

                    if (existingRecord != null)
                    {
                        // Nếu bản ghi tồn tại, cập nhật thông tin
                        existingRecord.TrangThai = item.TrangThai;
                        existingRecord.GhiChu = item.GhiChu;
                    }
                    else
                    {
                        // Nếu không tồn tại, thêm mới
                        var newRecord = new tblDiemDanh
                        {
                            MaSinhVienCuaLopHocPhan = item.MaSinhVienCuaLopHocPhan,
                            Buoi = item.Buoi,
                            NgayDiemDanh = item.NgayDiemDanh,
                            TrangThai = item.TrangThai,
                            GhiChu = item.GhiChu
                        };
                        _context.DiemDanhs.Add(newRecord);
                    }
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi lưu điểm danh: " + ex.Message });
            }
        }
        public class FormDataModel
        {
            public long MaLopHocPhan { get; set; }
            public int Buoi { get; set; }
            public DateTime NgayDiemDanh { get; set; }
        }

        public class SaveDataModel
        {
            public long MaSinhVienCuaLopHocPhan { get; set; }
            public int Buoi { get; set; }
            public DateTime NgayDiemDanh { get; set; }
            public string? TrangThai { get; set; }
            public string GhiChu { get; set; } = "";
        }
        [HttpGet]
        public async Task<IActionResult> GetHocKyByLopHocPhan(long maLopHocPhan)
        {
            var lopHocPhan = await _context.LopHocPhans
                .Where(lhp => lhp.MaLopHocPhan == maLopHocPhan)
                .Select(lhp => new { lhp.HocKy }) // Chỉ lấy trường HocKy
                .FirstOrDefaultAsync();

            if (lopHocPhan == null)
            {
                return Json(new { success = false, message = "Không tìm thấy lớp học phần." });
            }

            return Json(new { success = true, hocKy = lopHocPhan.HocKy });
        }
        
    }
}

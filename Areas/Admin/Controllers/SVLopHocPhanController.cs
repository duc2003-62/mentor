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
    public class SVLopHocPhanController : Controller
    {
        private readonly DataContext _context;
        public SVLopHocPhanController(DataContext context)
        {
            _context = context;
        }

        // Trang chính để xem tất cả lớp học phần
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách lớp học phần
            var lopHocPhanList = await _context.LopHocPhans.ToListAsync();
            if (lopHocPhanList == null)
            {
                return NotFound();
            }
            return View(lopHocPhanList);
        }

        // Xem chi tiết sinh viên đăng ký trong một lớp học phần
       public async Task<IActionResult> Details(long? id)
        {
            // Kiểm tra nếu id không hợp lệ
            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy mã lớp học phần.";
                return RedirectToAction("Index"); // Chuyển hướng về danh sách nếu không có id
            }

            // Lấy danh sách sinh viên đã đăng ký cho lớp học phần
            var sinhVienCuaLopHocPhanList = await _context.SinhVienCuaLopHocPhans
                .Include(sv => sv.SinhVien) // Lấy thông tin sinh viên
                .Where(lhp => lhp.MaLopHocPhan == id) // Lọc theo MaLopHocPhan
                .ToListAsync();

            // Lấy thông tin lớp học phần
            var lopHocPhan = await _context.LopHocPhans
                .FirstOrDefaultAsync(lhp => lhp.MaLopHocPhan == id);

            if (lopHocPhan == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy lớp học phần.";
                return RedirectToAction("Index"); // Chuyển hướng nếu không tìm thấy lớp học phần
            }

            // Lưu thông tin lớp học phần để hiển thị trong View
            ViewBag.LopHocPhanId = lopHocPhan.MaLopHocPhan;
            ViewBag.LopHocPhanName = lopHocPhan.TenLopHocPhan;

            // Trả về View với danh sách sinh viên (có thể rỗng)
            return View(sinhVienCuaLopHocPhanList);
        }


        [Route("Admin/svhocphan/create/{maLopHocPhan}")]
        public async Task<IActionResult> Create(long? maLopHocPhan)
        {
            if (maLopHocPhan == null)
            {
                return NotFound();
            }

            // Lấy danh sách tất cả sinh viên
            var allStudents = await _context.SinhViens.ToListAsync();

            // Lấy danh sách mã sinh viên đã có trong lớp học phần
            var studentsInClass = await _context.SinhVienCuaLopHocPhans
                                                .Where(s => s.MaLopHocPhan == maLopHocPhan)
                                                .Select(s => s.MaSinhVien)
                                                .ToListAsync();

            // // Lọc danh sách sinh viên chưa có trong lớp học phần
            // var availableStudents = allStudents
            //     .Where(s => !studentsInClass.Contains(s.MaSinhVien))
            //     .Select(s => new SelectListItem
            //     {
            //         //Value = s.MaSinhVien.ToString(),   // Sửa thành MaSinhVien (khóa chính)
            //         Value = s.MaSinhVienCode,
            //         Text = s.MaSinhVienCode,           // Text hiển thị vẫn là MaSinhVienCode
            //     })
            //     .ToList();
            // Lọc danh sách sinh viên chưa có trong lớp học phần
            var availableStudents = allStudents
                .Where(s => !studentsInClass.Contains(s.MaSinhVien)) // Lọc sinh viên chưa có trong lớp học phần
                .Select(s => new SelectListItem
                {
                    Value = s.MaSinhVien.ToString(),   // Gửi MaSinhVien (khóa chính) khi người dùng chọn
                    Text = s.MaSinhVienCode,           // Hiển thị MaSinhVienCode trong dropdown
                })
                .ToList();

            // Truyền danh sách sinh viên vào ViewBag
            ViewBag.AvailableStudents = availableStudents;

            // Truyền danh sách sinh viên vào ViewBag
            ViewBag.AvailableStudents = availableStudents;
            ViewBag.MaLopHocPhan = maLopHocPhan;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/svhocphan/create/{maLopHocPhan}")]
        public async Task<IActionResult> Create(tblSinhVienCuaLopHocPhan sinhVienCuaLopHocPhan)
        {
            Console.WriteLine("Đã nhận được yêu cầu POST.");

            // Kiểm tra ModelState
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState không hợp lệ:");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }

                // Lấy lại danh sách sinh viên khả dụng để render lại View
                var studentsInClass = await _context.SinhVienCuaLopHocPhans
                                                    .Where(s => s.MaLopHocPhan == sinhVienCuaLopHocPhan.MaLopHocPhan)
                                                    .Select(s => s.MaSinhVien)
                                                    .ToListAsync();

                var allStudents = await _context.SinhViens.ToListAsync();
                var availableStudents = allStudents
                    .Where(s => !studentsInClass.Contains(s.MaSinhVien))
                    .Select(s => new SelectListItem
                    {
                        Value = s.MaSinhVien.ToString(),
                        Text = s.MaSinhVienCode
                    })
                    .ToList();

                ViewBag.AvailableStudents = availableStudents;
                if (sinhVienCuaLopHocPhan.MaSinhVien > 0)
                {
                    var selectedStudent = await _context.SinhViens
                                                        .FirstOrDefaultAsync(s => s.MaSinhVien == sinhVienCuaLopHocPhan.MaSinhVien);
                    ViewBag.SelectedStudentName = selectedStudent?.HoTen;
                }
                ViewBag.MaLopHocPhan = sinhVienCuaLopHocPhan.MaLopHocPhan;

                return View("Create", sinhVienCuaLopHocPhan);
            }

            try
            {
                // Kiểm tra xem sinh viên đã có trong lớp học phần chưa
                var exists = await _context.SinhVienCuaLopHocPhans.AnyAsync(s =>
                    s.MaLopHocPhan == sinhVienCuaLopHocPhan.MaLopHocPhan &&
                    s.MaSinhVien == sinhVienCuaLopHocPhan.MaSinhVien);

                if (exists)
                {
                    ModelState.AddModelError(string.Empty, "Sinh viên này đã có trong lớp học phần.");
                    return RedirectToAction("Create", new { maLopHocPhan = sinhVienCuaLopHocPhan.MaLopHocPhan });
                }

                // Tạo bản ghi mới
                var newRecord = new tblSinhVienCuaLopHocPhan
                {
                    MaSinhVien = sinhVienCuaLopHocPhan.MaSinhVien,
                    MaLopHocPhan = sinhVienCuaLopHocPhan.MaLopHocPhan,
                    NgayDangKy = DateTime.Now, // Gán thời gian hiện tại
                    GhiChu = sinhVienCuaLopHocPhan.GhiChu
                };

                Console.WriteLine($"Thêm: MaSinhVien={newRecord.MaSinhVien}, MaLopHocPhan={newRecord.MaLopHocPhan}");

                // Thêm bản ghi vào cơ sở dữ liệu
                _context.Add(newRecord);
                await _context.SaveChangesAsync();

                Console.WriteLine("Thêm thành công.");
                TempData["SuccessMessage"] = "Sinh viên đã được thêm vào lớp học phần thành công!";

                return RedirectToAction("Details", new { id = sinhVienCuaLopHocPhan.MaLopHocPhan });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi lưu vào SQL: " + ex.Message);
                ModelState.AddModelError(string.Empty, "Không thể lưu vào cơ sở dữ liệu.");
                return View("Create", sinhVienCuaLopHocPhan);
            }
        }

        [HttpGet]
        [Route("api/search/{maSinhVien}")]
        public async Task<IActionResult> GetStudentInfo(long maSinhVien)
        {
            // Tìm sinh viên dựa trên MaSinhVien
            var sinhVien = await _context.SinhViens
                .FirstOrDefaultAsync(s => s.MaSinhVien == maSinhVien);

            // Nếu không tìm thấy sinh viên, trả về 404
            if (sinhVien == null)
            {
                return NotFound(new { message = "Không tìm thấy sinh viên." });
            }

            // Trả về thông tin sinh viên dưới dạng JSON
            return Ok(new
            {
                maSinhVien = sinhVien.MaSinhVien,
                hoTen = sinhVien.HoTen // Thuộc tính tên sinh viên
            });
        }






        // Code xóa sinh viên ra khỏi lớp học phần
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sinhVienCuaLopHocPhan = await _context.SinhVienCuaLopHocPhans
                .Include(sv => sv.SinhVien) // Lấy thông tin sinh viên liên quan
                .FirstOrDefaultAsync(m => m.MaSinhVienCuaLopHocPhan == id);

            if (sinhVienCuaLopHocPhan == null)
            {
                return NotFound();
            }

            return View(sinhVienCuaLopHocPhan); // Truyền dữ liệu đầy đủ cho view
        }


        // [HttpPost, ActionName("DeleteConfirmed")]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> DeleteConfirmed(long id)
        // {
        //     var sinhVienCuaLopHocPhan = await _context.SinhVienCuaLopHocPhans
        //         .FirstOrDefaultAsync(s => s.MaSinhVienCuaLopHocPhan == id);

        //     if (sinhVienCuaLopHocPhan != null)
        //     {
        //         _context.SinhVienCuaLopHocPhans.Remove(sinhVienCuaLopHocPhan);
        //         await _context.SaveChangesAsync();
        //     }

        //     return RedirectToAction("Index", "SVLopHocPhan", new { id = sinhVienCuaLopHocPhan?.MaLopHocPhan });
        // }
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            // Tìm sinh viên cần xóa
            var sinhVienCuaLopHocPhan = await _context.SinhVienCuaLopHocPhans
                .FirstOrDefaultAsync(s => s.MaSinhVienCuaLopHocPhan == id);

            if (sinhVienCuaLopHocPhan == null)
            {
                return NotFound();
            }

            // Kiểm tra xem sinh viên này có dữ liệu điểm danh liên quan không
            var hasAttendance = await _context.DiemDanhs
                .AnyAsync(dd => dd.MaSinhVienCuaLopHocPhan == id);

            if (hasAttendance)
            {
                // Nếu có dữ liệu điểm danh, thông báo lỗi
                TempData["ErrorMessage"] = "Không thể xóa sinh viên vì đã có dữ liệu điểm danh trong lớp học phần.";
                return RedirectToAction("Details", "SVLopHocPhan", new { id = sinhVienCuaLopHocPhan.MaLopHocPhan });
            }

            // Nếu không có dữ liệu điểm danh, tiến hành xóa
            _context.SinhVienCuaLopHocPhans.Remove(sinhVienCuaLopHocPhan);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Xóa sinh viên thành công.";
            return RedirectToAction("Details", "SVLopHocPhan", new { id = sinhVienCuaLopHocPhan.MaLopHocPhan });
        }

        // Phương thức Edit sinh viên của lớp học phần
        // GET: Admin/SVLopHocPhan/Edit/{id}
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Lấy thông tin sinh viên cần chỉnh sửa từ cơ sở dữ liệu
            var sinhVienCuaLopHocPhan = await _context.SinhVienCuaLopHocPhans
                .Include(sv => sv.SinhVien) // Lấy thông tin sinh viên liên quan
                .FirstOrDefaultAsync(s => s.MaSinhVienCuaLopHocPhan == id);

            if (sinhVienCuaLopHocPhan == null)
            {
                return NotFound();
            }

            return View(sinhVienCuaLopHocPhan); // Truyền dữ liệu vào view
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, tblSinhVienCuaLopHocPhan sinhVienCuaLopHocPhan)
        {
            Console.WriteLine($"Received ID: {id}");
            Console.WriteLine($"Model Data: GhiChu={sinhVienCuaLopHocPhan.GhiChu}, NgayDangKy={sinhVienCuaLopHocPhan.NgayDangKy}");

            if (id != sinhVienCuaLopHocPhan.MaSinhVienCuaLopHocPhan)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"Key: {error.Key}, Errors: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }
                return View(sinhVienCuaLopHocPhan);
            }

            try
            {
                var existingRecord = await _context.SinhVienCuaLopHocPhans
                    .FirstOrDefaultAsync(s => s.MaSinhVienCuaLopHocPhan == id);

                if (existingRecord == null)
                {
                    return NotFound();
                }

                existingRecord.GhiChu = sinhVienCuaLopHocPhan.GhiChu;
                existingRecord.NgayDangKy = sinhVienCuaLopHocPhan.NgayDangKy;

                // Đánh dấu bản ghi là đã thay đổi
                _context.Entry(existingRecord).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Thông tin sinh viên đã được cập nhật thành công!";
                return RedirectToAction("Details", new { id = existingRecord.MaLopHocPhan });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi cập nhật dữ liệu: " + ex.Message);
            }

            return View(sinhVienCuaLopHocPhan);
        }


  


        // POST: Admin/SVLopHocPhan/Edit/{id}
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Edit(long id, tblSinhVienCuaLopHocPhan sinhVienCuaLopHocPhan)
        // {

        //     Console.WriteLine($"Received ID: {id}");
        //     Console.WriteLine($"Model Data: GhiChu={sinhVienCuaLopHocPhan.GhiChu}, NgayDangKy={sinhVienCuaLopHocPhan.NgayDangKy}");
        //     if (id != sinhVienCuaLopHocPhan.MaSinhVienCuaLopHocPhan)
        //     {
        //         return NotFound();
        //     }
        //     if (!ModelState.IsValid)
        //     {
        //         foreach (var error in ModelState)
        //         {
        //             Console.WriteLine($"Key: {error.Key}, Errors: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
        //         }
        //         // Nếu dữ liệu không hợp lệ, trả về View cùng với dữ liệu để người dùng sửa
        //         return View(sinhVienCuaLopHocPhan);
        //     }

        //     try
        //     {
        //         // Cập nhật thông tin sinh viên trong lớp học phần
        //         var existingRecord = await _context.SinhVienCuaLopHocPhans
        //             .FirstOrDefaultAsync(s => s.MaSinhVienCuaLopHocPhan == id);

        //         if (existingRecord == null)
        //         {
        //             return NotFound();
        //         }

        //         existingRecord.GhiChu = sinhVienCuaLopHocPhan.GhiChu;
        //         existingRecord.NgayDangKy = sinhVienCuaLopHocPhan.NgayDangKy;

        //         _context.Update(existingRecord); // Đánh dấu bản ghi đã thay đổi
        //         await _context.SaveChangesAsync(); // Lưu vào cơ sở dữ liệu

        //         TempData["SuccessMessage"] = "Thông tin sinh viên đã được cập nhật thành công!";
        //         return RedirectToAction("Details", new { id = existingRecord.MaLopHocPhan });
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
        //         ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi cập nhật dữ liệu: " + ex.Message);
        //     }

        //     return View(sinhVienCuaLopHocPhan);
        // }
        
        
    }
}
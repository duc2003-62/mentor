using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mentor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml; // Import thư viện EPPlus
using OfficeOpenXml.Style; // Để định dạng Excel
using System.IO; // Để làm việc với stream
namespace mentor.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ThongKeDiemDanhController : Controller
    {
        private readonly DataContext _context;
        public ThongKeDiemDanhController(DataContext context)
        {
            _context = context;
        }

        // Trang chính để xem tất cả lớp học phần
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách lớp học phần từ cơ sở dữ liệu
            //var lopHocPhanList = await _context.LopHocPhans.ToListAsync();
            var lopHocPhanList = await _context.LopHocPhans
                .Include(lhp => lhp.GiangVien) // Bao gồm thông tin giảng viên
                .ToListAsync();

            // Nếu không có dữ liệu, trả về danh sách rỗng
            if (lopHocPhanList == null || !lopHocPhanList.Any())
            {
                TempData["ErrorMessage"] = "Không có lớp học phần nào.";
                return View(new List<tblLopHocPhan>()); // Trả về danh sách rỗng
            }

            // Trả về danh sách lớp học phần
            return View(lopHocPhanList);
        }

        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy mã lớp học phần.";
                return RedirectToAction("Index");
            }

            var lopHocPhan = await _context.LopHocPhans
                .Include(lhp => lhp.GiangVien) // Bao gồm thông tin giảng viên
                .FirstOrDefaultAsync(lhp => lhp.MaLopHocPhan == id);

            if (lopHocPhan == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy lớp học phần.";
                return RedirectToAction("Index");
            }

            // Tổng hợp dữ liệu thống kê
            var sinhVienThongKe = await (
                from svlhp in _context.SinhVienCuaLopHocPhans
                join sv in _context.SinhViens on svlhp.MaSinhVien equals sv.MaSinhVien
                join dd in _context.DiemDanhs on svlhp.MaSinhVienCuaLopHocPhan equals dd.MaSinhVienCuaLopHocPhan into ddGroup
                from dd in ddGroup.DefaultIfEmpty()
                where svlhp.MaLopHocPhan == id
                group dd by new { sv.MaSinhVienCode, sv.HoTen,sv.NgaySinh,sv.GioiTinh, svlhp.MaLopHocPhan } into g
                select new tblThongKeDiemDanhNew
                {
                    MaSinhVienCode = g.Key.MaSinhVienCode,
                    HoTen = g.Key.HoTen,
                    NgaySinh = g.Key.NgaySinh,
                    GioiTinh = g.Key.GioiTinh,
                    MaLopHocPhan = g.Key.MaLopHocPhan ?? 0, // Gán giá trị mặc định nếu null
                    SoBuoiCoMat = g.Count(d => d.TrangThai == "CM"),
                    SoBuoiVang = g.Count(d => d.TrangThai == "CP" || d.TrangThai == "KP"), // Tùy thuộc trạng thái vắng
                    SoBuoiDiMuon = g.Count(d => d.TrangThai == "M"),
                    TongSoBuoi = g.Count(d => d.TrangThai == "CM" || d.TrangThai == "M"), // Tổng buổi tham gia
                     // Tính điểm chuyên cần: 10 - số buổi vắng
                    // DiemChuyenCan = Math.Max(0, 10 - g.Count(d => d.TrangThai == "CP" || d.TrangThai == "KP"))
                    DiemChuyenCan = Math.Max(0, 10 - g.Count(d => d.TrangThai == "CP" || d.TrangThai == "KP") * 1 - g.Count(d => d.TrangThai == "M") * 0.5)

                }
            ).ToListAsync();
            ViewBag.LopHocPhan = lopHocPhan;
            return View(sinhVienThongKe);
            
        }
        public async Task<IActionResult> ExportToExcel(long id)
        {
            // Kiểm tra lớp học phần
            // var lopHocPhan = await _context.LopHocPhans
            //     .FirstOrDefaultAsync(lhp => lhp.MaLopHocPhan == id);
            var lopHocPhan = _context.LopHocPhans
            .Include(lhp => lhp.GiangVien)
            .FirstOrDefault(lhp => lhp.MaLopHocPhan == id);
            if (lopHocPhan == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy lớp học phần.";
                return RedirectToAction("Index");
            }

            // Lấy dữ liệu thống kê sinh viên
            var sinhVienThongKe = await (
                from svlhp in _context.SinhVienCuaLopHocPhans
                join sv in _context.SinhViens on svlhp.MaSinhVien equals sv.MaSinhVien
                join dd in _context.DiemDanhs on svlhp.MaSinhVienCuaLopHocPhan equals dd.MaSinhVienCuaLopHocPhan into ddGroup
                from dd in ddGroup.DefaultIfEmpty()
                where svlhp.MaLopHocPhan == id
                group dd by new { sv.MaSinhVienCode, sv.HoTen, sv.NgaySinh, sv.GioiTinh, svlhp.MaLopHocPhan } into g
                select new tblThongKeDiemDanhNew
                {
                    MaSinhVienCode = g.Key.MaSinhVienCode,
                    HoTen = g.Key.HoTen,
                    NgaySinh = g.Key.NgaySinh,
                    GioiTinh = g.Key.GioiTinh,
                    MaLopHocPhan = g.Key.MaLopHocPhan ?? 0,
                    SoBuoiCoMat = g.Count(d => d.TrangThai == "CM"),
                    SoBuoiDiMuon = g.Count(d => d.TrangThai == "M"),
                    SoBuoiVang = g.Count(d => d.TrangThai == "KP"),
                    TongSoBuoiThamGia = g.Count(d => d.TrangThai == "CM" || d.TrangThai == "M"),
                    TongSoBuoi = g.Count(),
                    DiemChuyenCan = Math.Max(0, 10 - g.Count(d => d.TrangThai == "CP" || d.TrangThai == "KP") * 1 - g.Count(d => d.TrangThai == "M") * 0.5),
                    
                }
            ).ToListAsync();

            // Tạo file Excel
            using (var package = new ExcelPackage())
            {
                var tenGiangVien = lopHocPhan.GiangVien?.HoTenGiangVien ?? "N/A";

                var worksheet = package.Workbook.Worksheets.Add("ThongKeDiemDanh");
                worksheet.Cells["A1"].Value = "Thống Kê Điểm Danh";
                worksheet.Cells["A1:J1"].Merge = true; // Gộp ô A1 đến J1
                worksheet.Cells["A1:J1"].Style.Font.Bold = true;
                worksheet.Cells["A1:J1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells["A2"].Value = $"Lớp học phần: {lopHocPhan.TenLopHocPhan}";
                worksheet.Cells["A2:J2"].Merge = true; // Gộp ô A2 đến J2
                worksheet.Cells["A2:J2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.Cells["A3:J3"].Merge = true; // Gộp ô A3 đến J3
                worksheet.Cells["A3:J3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells["A3:J3"].Value = $"Giảng viên: {tenGiangVien}";

                // Đặt tiêu đề cột
                var headers = new[]
                {
                    "STT", "Mã Sinh Viên", "Họ Tên", "Ngày Sinh", "Giới Tính",
                    "Số Buổi Có Mặt", "Số Buổi Đi Muộn", "Số Buổi Vắng",
                    "Tổng Số Buổi Tham Gia", "Tổng Số Buổi", "Điểm Chuyên Cần"
                };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[5, i + 1].Value = headers[i];
                    worksheet.Cells[5, i + 1].Style.Font.Bold = true;
                    worksheet.Cells[5, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                // Ghi dữ liệu vào Excel
                int row = 6;
                int stt = 1;
                foreach (var sinhVien in sinhVienThongKe)
                {
                    worksheet.Cells[row, 1].Value = stt++;
                    worksheet.Cells[row, 2].Value = sinhVien.MaSinhVienCode;
                    worksheet.Cells[row, 3].Value = sinhVien.HoTen;
                    worksheet.Cells[row, 4].Value = sinhVien.NgaySinh?.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 5].Value = sinhVien.GioiTinh;
                    worksheet.Cells[row, 6].Value = sinhVien.SoBuoiCoMat;
                    worksheet.Cells[row, 7].Value = sinhVien.SoBuoiDiMuon;
                    worksheet.Cells[row, 8].Value = sinhVien.SoBuoiVang;
                    worksheet.Cells[row, 9].Value = sinhVien.TongSoBuoiThamGia;
                    worksheet.Cells[row, 10].Value = sinhVien.TongSoBuoi;
                    worksheet.Cells[row, 11].Value = sinhVien.DiemChuyenCan; // Ghi điểm chuyên cần
                    row++;
                }

                // Định dạng bảng
                worksheet.Cells[5, 1, row - 1, 11].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[5, 1, row - 1, 11].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[5, 1, row - 1, 11].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[5, 1, row - 1, 11].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                // Căn chỉnh kích thước cột
                worksheet.Column(1).Width = 5; // Cột STT
                worksheet.Cells.AutoFitColumns();

                // Trả về file Excel
                var fileName = $"ThongKeDiemDanh_{lopHocPhan.TenLopHocPhan}.xlsx";
                var fileContent = package.GetAsByteArray();
                return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }
    }
}
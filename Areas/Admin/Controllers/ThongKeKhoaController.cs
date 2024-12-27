using System.Linq;
using mentor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace mentor.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ThongKeKhoaController : Controller
    {
        private readonly DataContext _context;

        public ThongKeKhoaController(DataContext context)
        {
            _context = context;
        }

        // Trang chọn loại thống kê
        public IActionResult Index()
        {
            return View();
        }

        // Thống kê danh sách các khoa hiện có
        public IActionResult ThongKeKhoaHienCo()
        {
            var danhSachKhoa = _context.Khoas
                .Select(k => new
                {
                    k.MaKhoa,
                    k.TenKhoa
                }).ToList();

            // Truyền danh sách vào View
            return View(danhSachKhoa);
        }

        // Thống kê danh sách các khoa với trưởng khoa hiện tại
        public IActionResult ThongKeTruongKhoa()
        {
            var danhSachTruongKhoa = _context.Khoas
                .Where(k => !string.IsNullOrEmpty(k.TruongKhoa))
                .Select(k => new
                {
                    k.MaKhoa,
                    k.TenKhoa,
                    k.TruongKhoa
                }).ToList();

            // Truyền danh sách vào View
            return View(danhSachTruongKhoa);
        }
        [HttpGet]
        public IActionResult ExportKhoaHienCo()
        {
            var danhSachKhoa = _context.Khoas
                .Select(k => new
                {
                    k.MaKhoa,
                    k.TenKhoa
                }).ToList();

            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Danh sách Khoa");

                // Thêm tiêu đề
                worksheet.Cell(1, 1).Value = "Mã Khoa";
                worksheet.Cell(1, 2).Value = "Tên Khoa";

                // Thêm dữ liệu
                for (int i = 0; i < danhSachKhoa.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = danhSachKhoa[i].MaKhoa;
                    worksheet.Cell(i + 2, 2).Value = danhSachKhoa[i].TenKhoa;
                }

                // Tạo bảng từ dữ liệu
                var range = worksheet.Range(1, 1, danhSachKhoa.Count + 1, 2); // Gồm tiêu đề và dữ liệu
                var table = range.CreateTable();
                table.Theme = ClosedXML.Excel.XLTableTheme.TableStyleMedium9; // Áp dụng theme cho bảng

                // Định dạng cột
                worksheet.Columns().AdjustToContents(); // Tự động điều chỉnh độ rộng các cột

                // Xuất file Excel
                using (var stream = new System.IO.MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhSachKhoaHienCo.xlsx");
                }
            }
        }

        [HttpGet]
        public IActionResult ExportTruongKhoa()
        {
            var danhSachTruongKhoa = _context.Khoas
                .Where(k => !string.IsNullOrEmpty(k.TruongKhoa))
                .Select(k => new
                {
                    k.MaKhoa,
                    k.TenKhoa,
                    k.TruongKhoa
                }).ToList();

            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Danh sách Trưởng Khoa");

                // Thêm dữ liệu vào worksheet
                worksheet.Cell(1, 1).Value = "Mã Khoa";
                worksheet.Cell(1, 2).Value = "Tên Khoa";
                worksheet.Cell(1, 3).Value = "Trưởng Khoa";

                for (int i = 0; i < danhSachTruongKhoa.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = danhSachTruongKhoa[i].MaKhoa;
                    worksheet.Cell(i + 2, 2).Value = danhSachTruongKhoa[i].TenKhoa;
                    worksheet.Cell(i + 2, 3).Value = danhSachTruongKhoa[i].TruongKhoa;
                }

                // Tạo bảng từ dữ liệu
                var range = worksheet.Range(1, 1, danhSachTruongKhoa.Count + 1, 3); // Bao gồm tiêu đề và dữ liệu
                var table = range.CreateTable();

                // Định dạng cột
                worksheet.Columns().AdjustToContents(); // Tự động điều chỉnh độ rộng các cột

                // Xuất file Excel
                using (var stream = new System.IO.MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhSachTruongKhoa.xlsx");
                }
            }
        }



    }
}

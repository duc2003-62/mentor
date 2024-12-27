using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;
using mentor.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace mentor.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ThongKeLopBienCheController : Controller
    {
        private readonly DataContext _context;

        public ThongKeLopBienCheController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var danhSachLopBienChe = _context.LopBienChes
                .Include(lb => lb.Khoa)
                .Select(lb => new
                {
                    lb.MaLop,
                    lb.TenLop,
                    lb.NamHoc,
                    Khoa = lb.Khoa!.TenKhoa
                })
                .OrderBy(lb => lb.Khoa)
                .ThenBy(lb => lb.NamHoc)
                .ToList();

            ViewBag.DanhSachLopBienChe = danhSachLopBienChe;

            return View();
        }


        // Thống kê danh sách các lớp biên chế theo từng khoa và từng năm học
        [HttpGet]
        public IActionResult ExportLopBienChe()
        {
            // Lấy danh sách lớp biên chế
            var danhSachLopBienChe = _context.LopBienChes
                .Include(lb => lb.Khoa) // Bao gồm thông tin khoa
                .Select(lb => new
                {
                    lb.MaLop,
                    lb.TenLop,
                    lb.NamHoc,
                    Khoa = lb.Khoa!.TenKhoa // Tên khoa từ bảng tblKhoa
                })
                .OrderBy(lb => lb.Khoa) // Sắp xếp theo tên khoa
                .ThenBy(lb => lb.NamHoc) // Sau đó sắp xếp theo năm học
                .ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Danh sách Lớp Biên Chế");

                // Thêm tiêu đề
                worksheet.Cell(1, 1).Value = "Mã Lớp";
                worksheet.Cell(1, 2).Value = "Tên Lớp";
                worksheet.Cell(1, 3).Value = "Khoa";
                worksheet.Cell(1, 4).Value = "Năm Học";

                // Thêm dữ liệu vào bảng
                for (int i = 0; i < danhSachLopBienChe.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = danhSachLopBienChe[i].MaLop;
                    worksheet.Cell(i + 2, 2).Value = danhSachLopBienChe[i].TenLop;
                    worksheet.Cell(i + 2, 3).Value = danhSachLopBienChe[i].Khoa;
                    worksheet.Cell(i + 2, 4).Value = danhSachLopBienChe[i].NamHoc;
                }

                // Tạo bảng từ dữ liệu
                var range = worksheet.Range(1, 1, danhSachLopBienChe.Count + 1, 4); // Bao gồm tiêu đề và dữ liệu
                var table = range.CreateTable();
                table.Theme = XLTableTheme.TableStyleMedium9; // Áp dụng theme cho bảng

                // Định dạng cột
                worksheet.Columns().AdjustToContents(); // Tự động điều chỉnh độ rộng các cột

                // Xuất file Excel
                using (var stream = new System.IO.MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DanhSachLopBienChe.xlsx");
                }
            }
        }
    }
}

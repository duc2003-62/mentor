// using System;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.Threading.Tasks;
// using mentor.Models;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.Rendering;

// namespace mentor.Areas.Admin.Controllers
// {
//     [Area("Admin")]
//     public class KhoaController : Controller
//     {
//          private readonly DataContext _context;
//         public KhoaController(DataContext context)
//         {
//             _context = context;
//         }
//         public IActionResult Index()
//         {
//             var mnList = _context.Khoas.OrderBy(m => m.MaKhoa).ToList();
//             return View(mnList);
//         }
//         public IActionResult Delete(long? id)
//         {
//             if (id == null || id == 0)
//                 return NotFound();
//             var mn= _context.Khoas.Find(id);
//             if (mn == null)
//                 return NotFound();
//             return View(mn);
//         }
//         [HttpPost]
//         public IActionResult Delete(long id)
//         {
//             var delKhoa = _context.Khoas.Find(id);
//             if (delKhoa == null)
//                 return NotFound();

//             // Kiểm tra xem có LopBienChe nào liên kết với Khoa này không
//             bool hasActiveLopBienChe = _context.LopBienChes.Any(lbc => lbc.MaKhoa == id);
            
//             // Kiểm tra xem có SinhVien nào liên kết với Khoa này không
//             bool hasActiveSinhVien = _context.SinhViens.Any(sv => sv.MaKhoa == id);
            
//             bool hasActiveGiangVien = _context.GiangViens.Any(gv => gv.MaGiangVien == id);

//             if (hasActiveLopBienChe || hasActiveSinhVien || hasActiveGiangVien)
//             {
//                 // Thêm thông báo lỗi vào ModelState
//                 ModelState.AddModelError("", "Không thể xóa Khoa này vì có lớp biên chế hoặc sinh viên đang hoạt động trong khoa này.");
//                 return View("Index", _context.Khoas.ToList()); // Quay về trang Index và hiển thị thông báo lỗi
//             }

//             _context.Khoas.Remove(delKhoa);
//             _context.SaveChanges();
//             return RedirectToAction("Index");
//         }
//         public IActionResult Create()
//         {
//             var mnList = (from m in _context.Khoas
//                           select new SelectListItem()
//                           {
//                             Value = m.MaKhoa.ToString() 
//                           }).ToList();
//             mnList.Insert(0, new SelectListItem()
//             {
//                 Text = "--- select ---",
//                 Value = "0"
//             });
//             ViewBag.mnList = mnList;
//             return View();
//         }
//         [HttpPost]
//         public IActionResult Create(tblKhoa mn)
//         {
//             if(ModelState.IsValid)
//             {
//                 mn.NgayTao = mn.NgayTao ?? DateTime.Now;
//                 _context.Khoas.Add(mn);
//                 _context.SaveChanges();
//                 return RedirectToAction("Index");
//             }
//             return View(mn);
//         }
//         public IActionResult Edit(long? id)
//         {
//             if (id == null || id == 0)
//                 return NotFound();
            
//             var mn = _context.Khoas.Find(id);
//             if (mn == null)
//                 return NotFound();

//             // Lấy danh sách mnList và đưa vào ViewBag để dùng trong view
//             var mnList = (from m in _context.Khoas
//                         select new SelectListItem()
//                         {
//                             Value = m.MaKhoa.ToString(),
//                             Text = m.TenKhoa  
//                         }).ToList();
//             mnList.Insert(0, new SelectListItem()
//             {
//                 Text = "--- select ---",
//                 Value = "0"
//             });
//             ViewBag.mnList = mnList;

//             return View(mn);
//         }

//         [HttpPost]
//         public IActionResult Edit(tblKhoa mn)
//         {
//             if (ModelState.IsValid)
//             {
//                 // Tải bản ghi cũ từ cơ sở dữ liệu để lấy giá trị `NgayTao` gốc
//                 var existingRecord = _context.Khoas.Find(mn.MaKhoa);
//                 if (existingRecord == null)
//                 {
//                     return NotFound();
//                 }

//                 // Giữ nguyên giá trị NgayTao hiện tại
//                 mn.NgayTao = existingRecord.NgayTao;

//                 // Cập nhật các trường khác từ mn vào existingRecord, ngoại trừ NgayTao
//                 existingRecord.TenKhoa = mn.TenKhoa;
//                 existingRecord.TruongKhoa = mn.TruongKhoa;
                
//                 // Cập nhật lại NgayTao với giá trị hiện tại nếu bạn muốn ghi đè nó
//                 existingRecord.NgayTao = DateTime.Now;
//                 // Lưu thay đổi
//                 _context.Khoas.Update(existingRecord);
//                 _context.SaveChanges();
                
//                 return RedirectToAction("Index");
//             }
            
//             // Nếu model không hợp lệ, trả về view với dữ liệu nhập
//             return View(mn);
//         }

//     }
// }
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using mentor.Models;

namespace mentor.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class KhoaController : Controller
    {
        private readonly DataContext _context;

        public KhoaController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var mnList = _context.Khoas.OrderBy(m => m.MaKhoa).ToList();
            return View(mnList);
        }

        public IActionResult Delete(long? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var mn = _context.Khoas.Find(id);
            if (mn == null)
                return NotFound();

            return View(mn);
        }

        [HttpPost]
        public IActionResult Delete(long id)
        {
            var delKhoa = _context.Khoas.Find(id);
            if (delKhoa == null)
                return NotFound();

            bool hasActiveLopBienChe = _context.LopBienChes.Any(lbc => lbc.MaKhoa == id);
            bool hasActiveSinhVien = _context.SinhViens.Any(sv => sv.MaKhoa == id);
            bool hasActiveGiangVien = _context.GiangViens.Any(gv => gv.MaGiangVien == id);

            if (hasActiveLopBienChe || hasActiveSinhVien || hasActiveGiangVien)
            {
                ModelState.AddModelError("", "Không thể xóa Khoa này vì có lớp biên chế hoặc sinh viên đang hoạt động trong khoa này.");
                return View("Index", _context.Khoas.ToList());
            }

            _context.Khoas.Remove(delKhoa);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Create()
        {
            var mnList = (from m in _context.Khoas
                          select new SelectListItem()
                          {
                              Value = m.MaKhoa.ToString()
                          }).ToList();
            mnList.Insert(0, new SelectListItem()
            {
                Text = "--- select ---",
                Value = "0"
            });
            ViewBag.mnList = mnList;
            return View();
        }

        [HttpPost]
        public IActionResult Create(tblKhoa mn)
        {
            // Kiểm tra trùng lặp Tên Khoa
            if (_context.Khoas.Any(k => k.TenKhoa == mn.TenKhoa))
            {
                ModelState.AddModelError("TenKhoa", "Tên Khoa đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                mn.NgayTao = mn.NgayTao ?? DateTime.Now;
                _context.Khoas.Add(mn);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mn);
        }

        public IActionResult Edit(long? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var mn = _context.Khoas.Find(id);
            if (mn == null)
                return NotFound();

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

            return View(mn);
        }

        [HttpPost]
        public IActionResult Edit(tblKhoa mn)
        {
            // Kiểm tra trùng lặp Tên Khoa (trừ khoa hiện tại đang chỉnh sửa)
            if (_context.Khoas.Any(k => k.TenKhoa == mn.TenKhoa && k.MaKhoa != mn.MaKhoa))
            {
                ModelState.AddModelError("TenKhoa", "Tên Khoa đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                var existingRecord = _context.Khoas.Find(mn.MaKhoa);
                if (existingRecord == null)
                {
                    return NotFound();
                }

                // Cập nhật các trường khác từ mn vào existingRecord
                existingRecord.TenKhoa = mn.TenKhoa;
                existingRecord.TruongKhoa = mn.TruongKhoa;

                _context.Khoas.Update(existingRecord);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(mn);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace mentor.Models
{
    [Table("tblLopHocPhan")]
    public class tblLopHocPhan
    {
        [Key]
        public long MaLopHocPhan { get; set; }

        [Required(ErrorMessage = "Tên lớp học phần không được để trống.")]
        [RegularExpression(@"^[\p{L}\p{M}0-9\s._]+$", ErrorMessage = "Tên lớp học phần không được chứa các ký tự đặc biệt")]
        [StringLength(100, ErrorMessage = "Tên học phần không được dài quá 100 ký tự.")]
        public string? TenLopHocPhan { get; set; }
        public long? MaHocPhan { get; set; }
        public long? MaGiangVien { get; set;}

        [Required(ErrorMessage = "Học kỳ không được để trống.")]
        [RegularExpression(@"^[\p{L}\p{M}0-9\s]+$", ErrorMessage = "Học kỳ không được chứa ký tự đặc biệt.")]
        public string? HocKy { get; set; }

        [Required(ErrorMessage = "Năm học không được để trống.")]
        [RegularExpression(@"^[\p{L}\p{M}0-9\s-]+$", ErrorMessage = "Năm học không được chứa ký tự đặc biệt.")]
        public string? NamHoc { get; set; }
        public DateTime? NgayBatDau { get; set; }  // Ngày bắt đầu
        public DateTime? NgayKetThuc { get; set; } // Ngày kết thúc

        public ICollection<tblSinhVienCuaLopHocPhan>? SinhVienCuaLopHocPhans { get; set; }
        
        [ForeignKey("MaHocPhan")]
        public tblHocPhan? HocPhan { get; set; }
        
        [ForeignKey("MaGiangVien")]
        public tblGiangVien? GiangVien { get; set; }
        
        /// <summary>
        /// Kiểm tra xem tên Khoa có trùng lặp trong cơ sở dữ liệu hay không.
        /// </summary>
        /// <param name="tenKhoa">Tên Khoa cần kiểm tra.</param>
        /// <param name="context">Context cơ sở dữ liệu.</param>
        /// <returns>True nếu tên Khoa đã tồn tại, ngược lại False.</returns>
        public static bool IsTenKhoaDuplicate(string tenlophocphan, DataContext context)
        {
            if (string.IsNullOrWhiteSpace(tenlophocphan))
            {
                throw new ArgumentException("Tên Khoa không được để trống.", nameof(tenlophocphan));
            }

            return context.LopHocPhans.Any(k => k.TenLopHocPhan == tenlophocphan);
        }
        
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace mentor.Models
{
    [Table("tblHocPhan")]
    public class tblHocPhan
    {
        [Key]
        public long MaHocPhan { get; set; }
        [Required(ErrorMessage = "Tên học phần không được để trống.")]
        [RegularExpression(@"^[\p{L}\p{M}0-9\s]+$", ErrorMessage = "Tên học phần không được chứa ký tự đặc biệt.")]
        [StringLength(100, ErrorMessage = "Tên học phần không được dài quá 100 ký tự.")]
        public string? TenHocPhan { get; set; }

        public int SoTinChi { get; set; }
        public long? MaKhoa { get; set; }

        public ICollection<tblLopHocPhan>? LopHocPhans { get; set; }
        
        [ForeignKey("MaKhoa")]
        public tblKhoa? Khoa { get; set; }
         /// <summary>
        /// Kiểm tra xem tên Khoa có trùng lặp trong cơ sở dữ liệu hay không.
        /// </summary>
        /// <param name="tenKhoa">Tên Khoa cần kiểm tra.</param>
        /// <param name="context">Context cơ sở dữ liệu.</param>
        /// <returns>True nếu tên Khoa đã tồn tại, ngược lại False.</returns>
        public static bool IsTenKhoaDuplicate(string tenhocphan, DataContext context)
        {
            if (string.IsNullOrWhiteSpace(tenhocphan))
            {
                throw new ArgumentException("Tên Khoa không được để trống.", nameof(tenhocphan));
            }

            return context.HocPhans.Any(k => k.TenHocPhan == tenhocphan);
        }
    }
}
// using System;
// using System.Collections.Generic;
// using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema;
// using System.Linq;
// using System.Threading.Tasks;

// namespace mentor.Models
// {
//     [Table("tblKhoa")]
//     public class tblKhoa
//     {
//         [Key]
//         public long MaKhoa { get; set; }

//         [Required(ErrorMessage = "Tên Khoa không được để trống.")]
//         [RegularExpression(@"^[\p{L}\p{M}0-9\s]+$", ErrorMessage = "Tên Khoa không được chứa ký tự đặc biệt.")]
//         public string? TenKhoa { get; set; }

//         [Required(ErrorMessage = "Trưởng Khoa không được để trống.")]
//         [RegularExpression(@"^[\p{L}\s]+$", ErrorMessage = "Trưởng Khoa chỉ được chứa chữ cái và khoảng trắng.")]
//         public string? TruongKhoa { get; set; }
//         public DateTime? NgayTao { get; set; }
        
//     }
// }
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mentor.Models
{
    [Table("tblKhoa")]
    public class tblKhoa
    {
        [Key]
        public long MaKhoa { get; set; }

        [Required(ErrorMessage = "Tên Khoa không được để trống.")]
        [RegularExpression(@"^[\p{L}\p{M}0-9\s]+$", ErrorMessage = "Tên Khoa không được chứa ký tự đặc biệt.")]
        [StringLength(100, ErrorMessage = "Tên Khoa không được dài quá 100 ký tự.")]
        public string? TenKhoa { get; set; }

        [Required(ErrorMessage = "Trưởng Khoa không được để trống.")]
        [RegularExpression(@"^[\p{L}\s]+$", ErrorMessage = "Trưởng Khoa chỉ được chứa chữ cái và khoảng trắng.")]
        [StringLength(100, ErrorMessage = "Tên Trưởng Khoa không được dài quá 100 ký tự.")]
        public string? TruongKhoa { get; set; }

        public DateTime? NgayTao { get; set; } = DateTime.Now;

        public ICollection<tblSinhVien>? SinhViens { get; set; }
        public ICollection<tblGiangVien>? GiangViens { get; set; }

        /// <summary>
        /// Kiểm tra xem tên Khoa có trùng lặp trong cơ sở dữ liệu hay không.
        /// </summary>
        /// <param name="tenKhoa">Tên Khoa cần kiểm tra.</param>
        /// <param name="context">Context cơ sở dữ liệu.</param>
        /// <returns>True nếu tên Khoa đã tồn tại, ngược lại False.</returns>
        public static bool IsTenKhoaDuplicate(string tenKhoa, DataContext context)
        {
            if (string.IsNullOrWhiteSpace(tenKhoa))
            {
                throw new ArgumentException("Tên Khoa không được để trống.", nameof(tenKhoa));
            }

            return context.Khoas.Any(k => k.TenKhoa == tenKhoa);
        }
    }
}

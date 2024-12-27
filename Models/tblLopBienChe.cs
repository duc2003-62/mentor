using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mentor.Models
{
    [Table("tblLopBienChe")]
    public class tblLopBienChe
    {
        [Key]
        public long MaLop { get; set; }

        [Required(ErrorMessage = "Tên Lớp không được để trống.")]
        [StringLength(100, ErrorMessage = "Tên Lớp không được dài quá 100 ký tự.")]
        public string? TenLop { get; set; }

        public long MaKhoa { get; set; }
        public int NamHoc { get; set; }

        [ForeignKey("MaKhoa")]
        public tblKhoa? Khoa { get; set; }

        /// <summary>
        /// Kiểm tra xem tên Lớp Biên Chế có trùng lặp trong cơ sở dữ liệu hay không.
        /// </summary>
        /// <param name="tenLop">Tên lớp cần kiểm tra.</param>
        /// <param name="context">Context cơ sở dữ liệu.</param>
        /// <returns>True nếu tên Lớp Biên Chế đã tồn tại, ngược lại False.</returns>
        public static bool IsTenLopDuplicate(string tenLop, DataContext context)
        {
            if (string.IsNullOrWhiteSpace(tenLop))
            {
                throw new ArgumentException("Tên Lớp không được để trống.", nameof(tenLop));
            }

            return context.LopBienChes.Any(l => l.TenLop == tenLop);
        }
    }
}

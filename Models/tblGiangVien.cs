using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace mentor.Models
{
    [Table("tblGiangVien")]
    public class tblGiangVien
    {
        [Key]
        public long MaGiangVien { get; set; }

        [Required(ErrorMessage = "Tên giảng viên không được để trống.")]
        [RegularExpression(@"^[\p{L}\p{M}0-9\s]+$", ErrorMessage = "Tên giảng viên không được chứa ký tự đặc biệt.")]
        public string? HoTenGiangVien { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string? GioiTinh { get; set; }

        [Required(ErrorMessage = "Email không được để trống.")]
        [RegularExpression(@"^[a-zA-Z0-9]+@gmail\.com$", ErrorMessage = "Email phải có định dạng hợp lệ và kết thúc bằng @gmail.com.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Số điện thoại phải có 11 chữ số.")]
        public string? SoDienThoai { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được để trống.")]
        [RegularExpression(@"^[\p{L}\p{M}0-9\s]+$", ErrorMessage = "Địa chỉ không được chứa ký tự đặc biệt.")]
        public string? DiaChi { get; set; }
        public long? MaKhoa { get; set; }
        
        [ForeignKey("MaKhoa")]
        public tblKhoa? Khoa { get; set; }

        public ICollection<tblLopHocPhan>? LopHocPhans { get; set; }
    }
}
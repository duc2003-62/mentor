using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace mentor.Models
{
    [Table("tblNguoiDung")]
    public class tblNguoiDung
    {
        [Key]
        public int MaNguoiDung { get; set; }

        [Required(ErrorMessage = "Tên tài khoản không được để trống.")]
        public string? TenTaiKhoan { get; set; }

        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]   
        [RegularExpression(@"^[a-zA-Z0-9]+@gmail\.com$", ErrorMessage = "Email phải có định dạng hợp lệ và kết thúc bằng @gmail.com.")] 
        public string? Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string? SoDienThoai { get; set; }
        public string? MatKhau { get; set; }
        public string? VaiTro { get; set; }
        public bool? TrangThai { get; set; }
        public DateTime? NgayTao { get; set; }
    }
}
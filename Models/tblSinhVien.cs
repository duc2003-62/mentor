using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace mentor.Models
{
    [Table("tblSinhVien")]
    public class tblSinhVien
    {
        [Key]
    public long MaSinhVien { get; set; }

    public string? MaSinhVienCode { get; set; }

    [Required(ErrorMessage = "Tên sinh viên không được để trống.")]
    [RegularExpression(@"^[\p{L}\p{M}0-9\s]+$", ErrorMessage = "Tên sinh viên không được chứa ký tự đặc biệt.")]
    public string? HoTen { get; set; }

    [Required(ErrorMessage = "Ngày sinh không được để trống.")]
    public DateTime? NgaySinh { get; set; }
    
    public string? GioiTinh { get; set; }

    
    [Required(ErrorMessage = "Lớp biên chế không được để trống.")]
    public long MaLop { get; set; }

    [Required(ErrorMessage = "Khoa không được để trống.")]
    public long? MaKhoa { get; set; } // Nullable foreign key

    [Required(ErrorMessage = "Email không được để trống.")]
    [RegularExpression(@"^[a-zA-Z0-9]+@gmail\.com$", ErrorMessage = "Email phải có định dạng hợp lệ và kết thúc bằng @gmail.com.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Số điện thoại không được để trống.")]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "Số điện thoại phải có 11 chữ số.")]
    public string? SoDienThoai { get; set; }

    [Required(ErrorMessage = "Địa chỉ không được để trống.")]
    [RegularExpression(@"^[\p{L}\p{M}0-9\s]+$", ErrorMessage = "Địa chỉ không được chứa ký tự đặc biệt.")]
    public string? DiaChi { get; set; }

    [ForeignKey("MaLop")]
    public tblLopBienChe? LopBienChe { get; set; }

    [ForeignKey("MaKhoa")]
    public tblKhoa? Khoa { get; set; }

    public ICollection<tblSinhVienCuaLopHocPhan>? SinhVienCuaLopHocPhans { get; set; }

    }
}
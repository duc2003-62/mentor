using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NuGet.Common;

namespace mentor.Areas.Admin.Models
{
    [Table("tblNguoiDung")]
    public class tblNguoiDung
    {
        [Key]
        public int MaNguoiDung { get; set; }
        public string? TenTaiKhoan { get; set; }
        public string? Email { get; set; }
        public string? SoDienThoai { get; set; }
        public string? MatKhau { get; set; }
        public string? VaiTro { get; set; }
        public bool? TrangThai { get; set; }
        public DateTime? NgayTao { get; set; }
        
    }
}
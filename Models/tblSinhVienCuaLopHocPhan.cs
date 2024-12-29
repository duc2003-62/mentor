using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace mentor.Models
{
    [Table("tblSinhVienCuaLopHocPhan")]
    public class tblSinhVienCuaLopHocPhan
    {
        [Key]
        public long MaSinhVienCuaLopHocPhan { get; set; }

        [Required(ErrorMessage = "Bạn phải chọn mã sinh viên.")]
        public long? MaSinhVien { get; set; }
        public long? MaLopHocPhan { get; set; }

        [Required(ErrorMessage = "Ngày đăng ký là bắt buộc.")]
        public DateTime? NgayDangKy { get; set; }
        public string? GhiChu { get; set; }
        
        [ForeignKey("MaSinhVien")]
        public tblSinhVien? SinhVien { get; set; }

        [ForeignKey("MaLopHocPhan")]
        public tblLopHocPhan? LopHocPhan { get; set; }
        public ICollection<tblThongKeDiemDanh>? ThongKeDiemDanhs { get; set; }
        public ICollection<tblDiemDanh>? DiemDanhs { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
namespace mentor.Models
{
    [Table("tblDiemDanh")]
    public class tblDiemDanh
    {
        [Key]
        public long MaDiemDanh { get; set; }  // Khóa chính của bảng tblDiemDanh

        public long? MaSinhVienCuaLopHocPhan { get; set; }  // Khóa ngoại liên kết tới tblSinhVien

        [Required(ErrorMessage = "Buổi không được để trống.")]
        public int Buoi { get; set; }
        public DateTime? NgayDiemDanh { get; set; }
        public string? TrangThai { get; set; }  // Trạng thái điểm danh như Có mặt, Vắng mặt, Đi muộn, v.v.
        public string? GhiChu { get; set; }
        
        [ForeignKey("MaSinhVienCuaLopHocPhan")]
        public tblSinhVienCuaLopHocPhan? SinhVienCuaLopHocPhan { get; set; }
    }
}
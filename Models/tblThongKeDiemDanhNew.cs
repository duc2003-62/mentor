using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
namespace mentor.Models
{
    [Table("tblThongKeDiemDanhNew")]
    public class tblThongKeDiemDanhNew
    {
        [Key]
        public string? MaSinhVienCode { get; set; }
        public string? HoTen { get; set; }
        public long MaLopHocPhan { get; set; } // Mã lớp học phần để thống kê theo lớp
        public DateTime? NgaySinh { get; set; }
        public string? GioiTinh { get; set; }
        public int SoBuoiCoMat { get; set; }
        public int SoBuoiVang { get; set; }
        public int SoBuoiDiMuon { get; set; }
        public int TongSoBuoi { get; set; }
        public int TongSoBuoiThamGia { get; set; }
        // Thêm điểm chuyên cần
        public double DiemChuyenCan { get; set; }
    }
}
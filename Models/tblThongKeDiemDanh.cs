using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace mentor.Models
{
    [Table("tblThongKeDiemDanh")]
    public class tblThongKeDiemDanh
    {
        [Key]
        public long MaThongKe { get; set; }
        public long? MaSinhVienCuaLopHocPhan { get; set; }
        public int SoBuoiCoMat { get; set; } = 0; // Số buổi có mặt, mặc định 0

        public int SoBuoiVang { get; set; } = 0; // Số buổi vắng, mặc định 0

        public int SoBuoiDiMuon { get; set; } = 0; // Số buổi đi muộn, mặc định 0

        public int TongSoBuoi { get; set; } = 0; // Tổng số buổi, mặc định 0
        
        [ForeignKey("MaSinhVienCuaLopHocPhan")]
        public tblSinhVienCuaLopHocPhan? SinhVienCuaLopHocPhan { get; set; }
    }
}
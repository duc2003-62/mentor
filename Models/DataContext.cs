using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using mentor.Areas.Admin.Models;
namespace mentor.Models
{
    public class DataContext : IdentityDbContext 
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<tblMenu> Menus { get; set; }
        public DbSet<tblKhoa> Khoas { get; set; }
        public DbSet<tblLopBienChe> LopBienChes { get; set; }
        public DbSet<tblGiangVien> GiangViens { get; set; }
        public DbSet<tblSinhVien> SinhViens { get; set; }
        public DbSet<tblHocPhan> HocPhans { get; set; }
        public DbSet<tblLopHocPhan> LopHocPhans { get; set; }
        public DbSet<tblSinhVienCuaLopHocPhan> SinhVienCuaLopHocPhans { get; set; }
        public DbSet<tblDiemDanh> DiemDanhs { get; set; }
        public DbSet<tblThongKeDiemDanh> ThongKeDiemDanhs { get; set; }
        public DbSet<tblThongKeDiemDanhNew> ThongKeDiemDanhNews { get; set; }
        public DbSet<tblNguoiDung> NguoiDungs  { get; set; }
        public DbSet<viewPostMenu> ViewPostMenus { get; set; }
        public DbSet<AdminMenu> adminMenus { get; set; }
    }
}
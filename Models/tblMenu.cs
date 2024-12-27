using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace mentor.Models
{
    [Table("tblMenu")]
    public class tblMenu
    {
        [Key]
        public int MenuID { get; set; }

        [Required(ErrorMessage = "Tên menu không được để trống.")]
        [RegularExpression(@"^[\p{L}\p{M}0-9\s]+$", ErrorMessage = "Tên menu không được chứa ký tự đặc biệt.")]
        public string? MenuName { get; set; }
        public string? ControllerName { get; set; }
        public string? ActionName { get; set; }
        public string? Link { get; set; }
        public int Levels { get; set; }
        public int ParentID { get; set; }
        public int MenuOrder { get; set; }
        public int Position { get; set; }
        public bool? IsActive { get; set; }
    }
}
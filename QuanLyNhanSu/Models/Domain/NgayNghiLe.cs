using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhanSu.Models.Domain
{
    [Table("NgayNghiLe")]
    public class NgayNghiLe
    {
        [Key]
        [Column("Ngay", TypeName = "date")]
        public DateTime Ngay { get; set; }

        [Required]
        [MaxLength(200)]
        public string TenNgayLe { get; set; } = string.Empty;
    }
}

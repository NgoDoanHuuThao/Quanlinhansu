using Microsoft.EntityFrameworkCore;
using QuanLyNhanSu.Models.Domain;

namespace QuanLyNhanSu.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<PhongBan> PhongBans { get; set; }

        public DbSet<ChucVu> ChucVus { get; set; }

        public DbSet<NhanVien> NhanViens { get; set; }

        public DbSet<TaiKhoan> TaiKhoans { get; set; }

        public DbSet<ChamCong> ChamCongs { get; set; }

        public DbSet<BangLuong> BangLuongs { get; set; }
        public DbSet<ThongBao> ThongBaos { get; set; }

        public DbSet<ThongBaoNguoiNhan> ThongBaoNguoiNhans { get; set; }
        public DbSet<DonNghiPhep> DonNghiPheps { get; set; }
        public DbSet<NgayNghiLe> NgayNghiLes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<NhanVien>()
                .HasOne(n => n.PhongBan)
                .WithMany(p => p.NhanViens)
                .HasForeignKey(n => n.PhongBanId);

            
            modelBuilder.Entity<NhanVien>()
                .HasOne(n => n.ChucVu)
                .WithMany(c => c.NhanViens)
                .HasForeignKey(n => n.ChucVuId);

            
            modelBuilder.Entity<TaiKhoan>()
                .HasOne(t => t.NhanVien)
                .WithMany(n => n.TaiKhoans)
                .HasForeignKey(t => t.NhanVienId);

            
            modelBuilder.Entity<ChamCong>()
                .HasOne(c => c.NhanVien)
                .WithMany(n => n.ChamCongs)
                .HasForeignKey(c => c.NhanVienId);

   
            modelBuilder.Entity<BangLuong>()
                .HasOne(b => b.NhanVien)
                .WithMany(n => n.BangLuongs)
                .HasForeignKey(b => b.NhanVienId);
            modelBuilder.Entity<ThongBaoNguoiNhan>()
    .HasOne(x => x.ThongBao)
    .WithMany(x => x.NguoiNhans)
    .HasForeignKey(x => x.ThongBaoId)
    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ThongBaoNguoiNhan>()
                .HasOne(x => x.NhanVien)
                .WithMany()
                .HasForeignKey(x => x.NhanVienId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ThongBaoNguoiNhan>()
                .HasIndex(x => new { x.ThongBaoId, x.NhanVienId })
                .IsUnique();
        }
    }
}

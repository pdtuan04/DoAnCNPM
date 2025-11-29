using Libs.Entity;
using Libs.Extensions;
using Libs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Libs
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<ChuDe> ChuDes { get; set; }
        public DbSet<LoaiBangLai> LoaiBangLais { get; set; }
        public DbSet<CauHoi> CauHois { get; set; }
        public DbSet<BaiThi> BaiThis { get; set; }
        public DbSet<ChiTietBaiThi> ChiTietBaiThis { get; set; }
        public DbSet<CauHoiSai> CauHoiSais { get; set; }

        public DbSet<BaiSaHinh> BaiSaHinhs { get; set; }
        public DbSet<LichSuThi> LichSuThis { get; set; }
        public DbSet<ChiTietLichSuThi> ChiTietLichSuThis { get; set; }
        public DbSet<Share> Shares { get; set; }
        public DbSet<ShareReply> ShareReplies { get; set; }
        public DbSet<ShareReport> ShareReports { get; set; }
        public DbSet<VisitLog> VisitLogs { get; set; }
        public DbSet<MoPhong> MoPhongs { get; set; }
        // phuong thuc thanh toan 
        public DbSet<DonHang> DonHangs { get; set; } = default!;
        public DbSet<GiaoDichThanhToan> GiaoDichThanhToans { get; set; } = default!;
        public DbSet<TinhNangMoKhoa> TinhNangMoKhoas { get; set; } = default!;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
            base.OnModelCreating(modelBuilder);
            Console.WriteLine("🚀 Application is seeding data...");
            modelBuilder.Entity<DonHang>()
                .Property(x => x.TongTien)
                .HasPrecision(18, 2);

            modelBuilder.Entity<TinhNangMoKhoa>()
                .Property(x => x.SoTienDaTra)
                .HasPrecision(18, 2);


            modelBuilder.Entity<TinhNangMoKhoa>()
                .HasIndex(x => new { x.UserId, x.TenTinhNang, x.DangHoatDong })
                .HasDatabaseName("UX_User_TinhNang_Active");

            modelBuilder.Entity<GiaoDichThanhToan>()
                .HasIndex(x => x.MaDonCong);

            modelBuilder.Entity<GiaoDichThanhToan>()
                .HasIndex(x => x.MaGiaoDichCuoi)
                .IsUnique();

            modelBuilder.Entity<GiaoDichThanhToan>()
                .HasIndex(x => new { x.TrangThai, x.NgayTao });
            modelBuilder.Entity<DonHang>()
                .Property(x => x.NgayTao)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<GiaoDichThanhToan>()
                .Property(x => x.NgayTao)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<TinhNangMoKhoa>()
                .Property(x => x.NgayTao)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.SeedingData();
          
        }
    }
}

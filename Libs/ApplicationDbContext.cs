﻿using Libs.Entity;
using Libs.Models;
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
        public DbSet<Sharereport> ShareReports { get; set; }
        public DbSet<GiaoDich> GiaoDichs { get; set; } // Thêm DbSet cho GiaoDich
        public DbSet<VisitLog> VisitLogs { get; set; }
        public DbSet<MoPhong> MoPhongs { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}

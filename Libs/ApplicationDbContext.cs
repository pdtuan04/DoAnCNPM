using Libs.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Libs
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Questions> Questions { get; set; }
        public DbSet<ChuDes> ChuDes { get; set; }
        public DbSet<LoaiBangLais> LoaiBangLais { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}

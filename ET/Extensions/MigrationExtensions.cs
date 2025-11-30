using Libs;
using Microsoft.EntityFrameworkCore;

namespace ET.Extensions
{
    public static class MigrationExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();
            var dbcontext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var hasMigrations = dbcontext.Database.GetAppliedMigrations().Any();
            if (!hasMigrations)
            {
                dbcontext.Database.Migrate();
            }
            //using IServiceScope scope = app.ApplicationServices.CreateScope();
            //var dbcontext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            //try
            //{
            //    var pendingMigrations = dbcontext.Database.GetPendingMigrations();
            //    if (pendingMigrations.Any())
            //    {
            //        Console.WriteLine($"⚠️ Found {pendingMigrations.Count()} pending migrations.  Applying.. .");
            //        dbcontext.Database.Migrate();
            //    }
            //    else
            //    {
            //        Console.WriteLine("✅ Database is up to date.");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"❌ Migration failed: {ex.Message}");
            //    throw;
            //}
        }
    }
}

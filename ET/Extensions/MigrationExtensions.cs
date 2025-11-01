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
            dbcontext.Database.Migrate();
        }
    }
}

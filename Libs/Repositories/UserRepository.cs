using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libs.Data;
using Libs.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Libs.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        public Task<int> GetAllUserCountByMonthAsync();
        public Task<Dictionary<int, int>> GetUserCountByMonthInCurrentYearAsync();

    }
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<int> GetAllUserCountByMonthAsync()
        {
            var thisMonth = DateTime.Now.Month;
            int userCountByMonth = await _dbContext.Users
                .Where(u => u.CreatedAt.Month == thisMonth)
                .CountAsync();
            return userCountByMonth;
        }
        // Implementation
        public async Task<Dictionary<int, int>> GetUserCountByMonthInCurrentYearAsync()
        {
            var currentYear = DateTime.Now.Year;

            var counts = await _dbContext.Users
                .Where(u => u.CreatedAt.Year == currentYear)
                .GroupBy(u => u.CreatedAt.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() })
                .ToListAsync();
            var result = Enumerable.Range(1, 12)
                .ToDictionary(
                    m => m,
                    m => counts.FirstOrDefault(x => x.Month == m)?.Count ?? 0
                );

            return result;
        }
        public async Task<int> GetAllUsersCountAsync()
        {
            return await _dbContext.Users.CountAsync();
        }
    }
}

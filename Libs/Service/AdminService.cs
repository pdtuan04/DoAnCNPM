using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libs.Data;
using Libs.Entity;
using Libs.Repositories;

namespace Libs.Service
{
    public class AdminService
    {
        private ApplicationDbContext dbContext;
        private IMoPhongRepository moPhongRepository;
        private UserRepository userRepository;
        public AdminService(ApplicationDbContext dbContext) {
            this.dbContext = dbContext;
            this.moPhongRepository = new MoPhongRepository(dbContext);
            this.userRepository = new UserRepository(dbContext);
        }
        public void Save()
        {
            dbContext.SaveChanges();
        }
        public async Task<PageList<MoPhong>> GetPagedMoPhong(int pageNumber, int pageSize, string? search, string? sortCol, string sortDir)
        {
            return await moPhongRepository.GetPagedMoPhong(pageNumber, pageSize, search, sortCol, sortDir);
        }
        public async Task<int> GetTotalUserCountAsync()
        {
            return await userRepository.GetAllUserCountByMonthAsync();
        }
        public async Task<Dictionary<int, int>> GetUserCountByMonthInCurrentYearAsync()
        {
            return await userRepository.GetUserCountByMonthInCurrentYearAsync();
        }
        public async Task<int> GetAllUsersCountAsync()
        {
            return await userRepository.GetAllUsersCountAsync();
        }
    }
}

using Libs.Data;
using Libs.Entity;
using Libs;
using Microsoft.EntityFrameworkCore;
namespace Libs.Repositories
{
    public interface IChuDeRepository : IRepository<ChuDes>
    {
    }

    public class ChuDeRepository : RepositoryBase<ChuDes>, IChuDeRepository
    {
        public ChuDeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
using Libs.Data;
using Libs.Entity;
using Libs;
namespace Libs.Repositories
{
    public interface ILoaiBangLaiRepository : IRepository<LoaiBangLais>
    {
    }

    public class LoaiBangLaiRepository : RepositoryBase<LoaiBangLais>, ILoaiBangLaiRepository
    {
        public LoaiBangLaiRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
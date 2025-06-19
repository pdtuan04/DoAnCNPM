using Libs.Data;
using Libs.Entity;
using Libs;
namespace Libs.Repositories
{
    public interface IMoPhongRepository : IRepository<MoPhong>
    {
        IEnumerable<MoPhong> GetMoPhongsByLoaiBangLaiId(Guid loaId);
        IEnumerable<MoPhong> GetAllMoPhong();
        MoPhong GetMoPhongById(Guid id);
    }

    public class MoPhongRepository : RepositoryBase<MoPhong>, IMoPhongRepository
    {
        public MoPhongRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public IEnumerable<MoPhong> GetMoPhongsByLoaiBangLaiId(Guid loaiBangLaiId)
        {
            return _dbContext.MoPhongs.Where(x => x.LoaiBangLaiId == loaiBangLaiId)
                                      .ToList();
        }
        public IEnumerable<MoPhong> GetAllMoPhong()
        {
            return _dbContext.MoPhongs.ToList();
        }
        public MoPhong GetMoPhongById(Guid id)
        {
            return _dbContext.MoPhongs.FirstOrDefault(x => x.Id == id);
        }
    }
}
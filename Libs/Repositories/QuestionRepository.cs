using Libs.Data;
using Libs.Entity;
using Libs;
using Microsoft.EntityFrameworkCore;

namespace Libs.Repositories
{
    public interface IQuestionRepository : IRepository<Questions>
    {
        IEnumerable<Questions> GetQuestionsByLoaiBangLai(Guid loaiBangLaiId, int count);
        IEnumerable<Questions> GetQuestionByChuDe(Guid chuDeId, int count);
    }

    public class QuestionRepository : RepositoryBase<Questions>, IQuestionRepository
    {
        public QuestionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public IEnumerable<Questions> GetQuestionsByLoaiBangLai(Guid loaiBangLaiId, int count)
        {
            return _dbSet.Where(q => q.LoaiBangLaiId == loaiBangLaiId && !q.IsDelete)
                .OrderBy(q => Guid.NewGuid())
                .Take(count)
                .ToList();
        }

        public IEnumerable<Questions> GetQuestionByChuDe(Guid chuDeId, int count)
        {
            return _dbSet.Where(q => q.ChuDeId == chuDeId && !q.IsDelete)
                .OrderBy(q => Guid.NewGuid())
                .Take(count)
                .ToList();
        }
    }
}
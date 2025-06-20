using Libs;
using Libs.Data;
using Libs.Entity;
using Microsoft.EntityFrameworkCore;
namespace Libs.Repositories
{
    public interface ILoaiBangLaiRepository : IRepository<LoaiBangLai>
    {
        Task<List<LoaiBangLai>> GetLoaiBangLaiXeMayAsync();
        Task<List<LoaiBangLai>> GetLoaiBangLaiOToAsync();
        Task<List<LoaiBangLai>> GetDanhSachLoaiBangLaiAsync();
        Task<LoaiBangLai> GetLoaiBangLaiByIdAsync(Guid loaiBangLaiId);
        Task<(LoaiBangLai, List<ChuDe>)> GetChuDeByLoaiBangLaiAsync(Guid loaiBangLaiId);
    }

    public class LoaiBangLaiRepository : RepositoryBase<LoaiBangLai>, ILoaiBangLaiRepository
    {
        public LoaiBangLaiRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<LoaiBangLai>> GetLoaiBangLaiXeMayAsync()
        {
            return await _dbContext.LoaiBangLais
                .Where(l => l.LoaiXe == "Xe máy" && !l.isDeleted)
                .ToListAsync();
        }

        public async Task<List<LoaiBangLai>> GetLoaiBangLaiOToAsync()
        {
            return await _dbContext.LoaiBangLais
                .Where(l => l.LoaiXe == "Xe oto" && !l.isDeleted)
                .ToListAsync();
        }

        public async Task<List<LoaiBangLai>> GetDanhSachLoaiBangLaiAsync()
        {
            return await _dbContext.LoaiBangLais
                .Where(l => !l.isDeleted)
                .ToListAsync();
        }

        public async Task<LoaiBangLai> GetLoaiBangLaiByIdAsync(Guid loaiBangLaiId)
        {
            return await _dbContext.LoaiBangLais
                .FirstOrDefaultAsync(l => l.Id == loaiBangLaiId && !l.isDeleted);
        }

        public async Task<(LoaiBangLai, List<ChuDe>)> GetChuDeByLoaiBangLaiAsync(Guid loaiBangLaiId)
        {
            var loai = await _dbContext.LoaiBangLais
                .FirstOrDefaultAsync(l => l.Id == loaiBangLaiId && !l.isDeleted);

            var chuDeList = await _dbContext.ChuDes
                .Include(cd => cd.CauHois)
                .Where(cd => !cd.isDeleted &&
                             cd.CauHois.Any(ch => ch.LoaiBangLaiId == loaiBangLaiId &&
                                                  !ch.LoaiBangLai.isDeleted &&
                                                  !ch.ChuDe.isDeleted))
                .Distinct()
                .ToListAsync();

            return (loai, chuDeList);
        }
    }
}
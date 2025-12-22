using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libs.Data;
using Libs.Entity;
using Libs.Service;
using Microsoft.EntityFrameworkCore;

namespace Libs.Repositories
{
    public interface ICauHoiRepository : IRepository<CauHoi>
    {
        Task<List<CauHoi>> GetAllCauHoiAsync();
        Task<CauHoi> GetCauHoiByIdAsync(Guid id);
        public Task<PageList<CauHoi>> GetPagedCauHoi(int pageNumber, int pageSize, string? search, string? sortCol, string? sortDir);
        Task<List<CauHoi>> CauHoiHaySai(int soLuong);
        Task<PageList<CauHoi>> GetCauHoisOnTapTheoChuDeAsync(int pageNumber, int pageSize, string? search, string? sortCol, string sortDir, Guid? chuDeId, Guid? LoaiBangLaiId);
        Task<List<CauHoi>> GetCauHoiNgauNhien(int soLuong);
        Task<List<CauHoi>> GetCauHoiNgauNhienTheoChuDe(Guid? loaiBangLaiId, Guid chuDeId, int soLuong);
    }

    public class CauHoiRepository : RepositoryBase<CauHoi>, ICauHoiRepository
    {
        public CauHoiRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<List<CauHoi>> GetAllCauHoiAsync()
        {
            return await _dbContext.CauHois
            .Where(x => x.isDeleted == false)
            .ToListAsync();
        }

        public async Task<CauHoi> GetCauHoiByIdAsync(Guid id)
        {
            return await _dbContext.CauHois.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<PageList<CauHoi>> GetPagedCauHoi(int pageNumber, int pageSize, string? search, string? sortCol, string? sortDir)
        {
            IQueryable<CauHoi> CauHoiQuery = _dbContext.CauHois.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                CauHoiQuery = CauHoiQuery.Where(p => p.NoiDung.Contains(search));
            }
            CauHoiQuery = CauHoiQuery.Where(x => !x.isDeleted);
            var entityProps = typeof(CauHoi)
            .GetProperties()
            .ToDictionary(p => p.Name.ToLower(), p => p.Name, StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrWhiteSpace(sortCol) && entityProps.ContainsKey(sortCol))
            {
                string actualCol = entityProps[sortCol];
                bool isDescending = sortDir?.Equals("desc", StringComparison.OrdinalIgnoreCase) ?? false;

                CauHoiQuery = isDescending
                    ? CauHoiQuery.OrderByDescending(q => EF.Property<object>(q, actualCol))
                    : CauHoiQuery.OrderBy(q => EF.Property<object>(q, actualCol));
            }
            else
            {
                CauHoiQuery = CauHoiQuery.OrderBy(q => q.Id);
            }
            var CauHois = await PageList<CauHoi>.CreatePageAsync(CauHoiQuery, pageNumber, pageSize);
            return CauHois;
        }
        public async Task<List<CauHoi>> CauHoiHaySai(int soLuong)
        {
            var result = await _dbContext.ChiTietLichSuThis
                .Where(c => c.DungSai == true)
                .GroupBy(c => c.CauHoiId)
                .Select(c => new
                {
                    CauHoiId = c.Key,
                    SaiCount = c.Count()
                })
                .OrderByDescending(c => c.SaiCount)
                .Take(soLuong)
                .Select(c => c.CauHoiId)
                .Join(_dbContext.CauHois,
                        id => id,
                        cauHoi => cauHoi.Id,
                        (id, cauHoi) => cauHoi
                ).ToListAsync();
            return result;
        }
        public async Task<PageList<CauHoi>> GetCauHoisOnTapTheoChuDeAsync(int pageNumber, int pageSize, string? search, string? sortCol, string sortDir, Guid? chuDeId, Guid? LoaiBangLaiId)
        {
            IQueryable<CauHoi> CauHoiQuery = _dbContext.CauHois.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                CauHoiQuery = CauHoiQuery.Where(p => p.NoiDung.Contains(search));
            }
            CauHoiQuery = CauHoiQuery.Where(x => !x.isDeleted);

            if (LoaiBangLaiId.HasValue && LoaiBangLaiId.Value != Guid.Empty)
            {
                CauHoiQuery = CauHoiQuery.Where(x => x.LoaiBangLaiId == LoaiBangLaiId);
            }

            if (chuDeId.HasValue && chuDeId.Value != Guid.Empty)
            {
                CauHoiQuery = CauHoiQuery.Where(x => x.ChuDeId == chuDeId);
            }
            var entityProps = typeof(CauHoi)
            .GetProperties()
            .ToDictionary(p => p.Name.ToLower(), p => p.Name, StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrWhiteSpace(sortCol) && entityProps.ContainsKey(sortCol))
            {
                string actualCol = entityProps[sortCol];
                bool isDescending = sortDir?.Equals("desc", StringComparison.OrdinalIgnoreCase) ?? false;

                CauHoiQuery = isDescending
                    ? CauHoiQuery.OrderByDescending(q => EF.Property<object>(q, actualCol))
                    : CauHoiQuery.OrderBy(q => EF.Property<object>(q, actualCol));
            }
            else
            {
                CauHoiQuery = CauHoiQuery.OrderBy(q => q.Id);
            }
            var CauHois = await PageList<CauHoi>.CreatePageAsync(CauHoiQuery, pageNumber, pageSize);
            return CauHois;
        }
        public async Task<List<CauHoi>> GetCauHoiNgauNhien(int soLuong)
        {
            var cauHois = await _dbContext.CauHois
                .OrderBy(c => Guid.NewGuid())
                .Take(soLuong)
                .ToListAsync();
            return cauHois;
        }
        public async Task<List<CauHoi>> GetCauHoiNgauNhienTheoChuDe(Guid? loaiBangLaiId, Guid chuDeId, int soLuong)
        {
            var query = _dbContext.CauHois
                        .Where(c => c.ChuDeId == chuDeId);

            if (loaiBangLaiId.HasValue)
            {
                query = query.Where(c => c.LoaiBangLaiId == loaiBangLaiId.Value);
            }

            var result = await query
                .OrderBy(c => Guid.NewGuid())
                .Take(soLuong)
                .ToListAsync();

            return result;
        }
    }
}

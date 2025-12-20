using Libs.Data;
using Libs.Entity;
using Libs.Models;
using Libs.Service; // PageList
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Libs.Repositories
{
    public interface ILichSuThiRepository : IRepository<LichSuThi>
    {
        Task<PageList<LichSuThi>> GetLichSuThiByUserAsync(string userId, int pageNumber = 1, int pageSize = 10);
        Task<LichSuThiDetailModel?> GetLichSuThiDetailAsync(Guid lichSuThiId);
        Task<LichSuThiStatModel> GetLichSuThiStatsAsync(string userId);
        Task<List<CauHoiSaiFrequencyModel>> GetFrequentWrongQuestionsAsync(string userId, int limit = 10);
        Task<bool> DeleteLichSuThiAsync(Guid lichSuThiId, string userId);
        Task<bool> SaveLichSuThiAsync(LichSuThi lichSuThi, List<ChiTietLichSuThi> chiTietList);
        ApplicationDbContext GetDbContext();

        Task<List<CauHoi>> GetCauHoiSaiByUserAsync(string userId);
        Task<CauHoi?> GetCauHoiByIdAsync(Guid id);
        Task<List<CauHoiSai>> GetCauHoiSaiListAsync(string userId, Guid cauHoiId);
        Task XoaCauHoiSaisAsync(List<CauHoiSai> list);
        Task AddCauHoiSaiAsync(CauHoiSai cauHoiSai);
        Task SaveChangesAsync();
        Task<PageList<LichSuThi>> GetLichSuThiByUserAsync(string userId, int pageNumber = 1, int pageSize = 10, string? result = null);

    }

    public class LichSuThiRepository : RepositoryBase<LichSuThi>, ILichSuThiRepository
    {
        public LichSuThiRepository(ApplicationDbContext context) : base(context) { }


        public async Task<PageList<LichSuThi>> GetLichSuThiByUserAsync(string userId, int pageNumber = 1, int pageSize = 10)
        {
            var query = _dbContext.LichSuThis
                .Where(ls => ls.UserId == userId)
                .OrderByDescending(ls => ls.NgayThi)
                .AsQueryable();

            return await PageList<LichSuThi>.CreatePageAsync(query, pageNumber, pageSize);
        }
        public async Task<PageList<LichSuThi>> GetLichSuThiByUserAsync(string userId, int pageNumber = 1, int pageSize = 10, string? result = null)
        {
            var query = _dbContext.LichSuThis
                .AsNoTracking()
                .Where(ls => ls.UserId == userId);

            var r = (result ?? "").Trim().ToLowerInvariant();
            if (!string.IsNullOrEmpty(r) && r != "all")
            {
                if (r is "pass" or "passed" or "dau")
                    query = query.Where(ls => ls.KetQua == "Đậu");
                else if (r is "fail" or "failed" or "rot")
                    query = query.Where(ls => ls.KetQua != "Đậu"); // rớt/không đạt
            }

            query = query.OrderByDescending(ls => ls.NgayThi);
            return await PageList<LichSuThi>.CreatePageAsync(query, pageNumber, pageSize);
        }

        public async Task<LichSuThiDetailModel?> GetLichSuThiDetailAsync(Guid lichSuThiId)
        {
            var lichSuThi = await _dbContext.LichSuThis
                .FirstOrDefaultAsync(ls => ls.Id == lichSuThiId);

            if (lichSuThi == null)
                return null;

            var chiTietList = await _dbContext.ChiTietLichSuThis
                .Where(ct => ct.LichSuThiId == lichSuThiId)
                .Include(ct => ct.CauHoi)
                    .ThenInclude(c => c.ChuDe)
                .Include(ct => ct.CauHoi)
                    .ThenInclude(c => c.LoaiBangLai)
                .ToListAsync();

            var baiThi = await _dbContext.BaiThis
                .Include(b => b.ChiTietBaiThis)
                .FirstOrDefaultAsync(b => b.Id == lichSuThi.BaiThiId);

            return new LichSuThiDetailModel
            {
                LichSuThi = lichSuThi,
                ChiTietList = chiTietList,
                BaiThi = baiThi
            };
        }

        public async Task<LichSuThiStatModel> GetLichSuThiStatsAsync(string userId)
        {
            var allHistory = await _dbContext.LichSuThis
                .Where(ls => ls.UserId == userId)
                .ToListAsync();

            if (!allHistory.Any())
                return new LichSuThiStatModel();

            return new LichSuThiStatModel
            {
                TongSoBaiThi = allHistory.Count,
                SoBaiThiDat = allHistory.Count(ls => ls.KetQua == "Đậu"),
                SoBaiThiKhongDat = allHistory.Count(ls => ls.KetQua != "Đậu"),
                DiemTrungBinh = allHistory.Average(ls => ls.Diem),
                TyLeDung = allHistory.Average(ls => ls.PhanTramDung / 10),
                BaiThiGanNhat = allHistory.OrderByDescending(ls => ls.NgayThi).FirstOrDefault()
            };
        }

        // ✅ SỬA/TỐI ƯU: đếm đúng + tránh N+1 query
        public async Task<List<CauHoiSaiFrequencyModel>> GetFrequentWrongQuestionsAsync(string userId, int limit = 10)
        {
            var list = await _dbContext.CauHoiSais
                .AsNoTracking()
                .Where(cs => cs.UserId == userId)
                .OrderByDescending(cs => cs.SoLanSai)
                .ThenByDescending(cs => cs.NgaySai)
                .Take(limit)
                .Select(cs => new CauHoiSaiFrequencyModel
                {
                    CauHoiId = cs.CauHoiId,
                    SoLanSai = cs.SoLanSai,
                    NgaySaiGanNhat = cs.NgaySai
                })
                .ToListAsync();

            var ids = list.Select(x => x.CauHoiId).ToList();
            if (ids.Count == 0) return list;

            var questions = await _dbContext.CauHois
                .AsNoTracking()
                .Include(c => c.ChuDe)
                .Include(c => c.LoaiBangLai)
                .Where(c => ids.Contains(c.Id))
                .ToDictionaryAsync(c => c.Id);

            foreach (var item in list)
                if (questions.TryGetValue(item.CauHoiId, out var q))
                    item.CauHoi = q;

            return list;
        }


        public async Task<bool> DeleteLichSuThiAsync(Guid lichSuThiId, string userId)
        {
            try
            {
                var lichSuThi = await _dbContext.LichSuThis
                    .FirstOrDefaultAsync(ls => ls.Id == lichSuThiId && ls.UserId == userId);

                if (lichSuThi == null)
                    return false;

                using (var transaction = await _dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var chiTietList = await _dbContext.ChiTietLichSuThis
                            .Where(ct => ct.LichSuThiId == lichSuThiId)
                            .ToListAsync();

                        _dbContext.ChiTietLichSuThis.RemoveRange(chiTietList);
                        _dbContext.LichSuThis.Remove(lichSuThi);

                        await _dbContext.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return true;
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting exam history: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SaveLichSuThiAsync(LichSuThi lichSuThi, List<ChiTietLichSuThi> chiTietList)
        {
            try
            {
                using (var transaction = await _dbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        await _dbContext.LichSuThis.AddAsync(lichSuThi);
                        await _dbContext.SaveChangesAsync();

                        foreach (var chiTiet in chiTietList)
                        {
                            chiTiet.LichSuThiId = lichSuThi.Id;
                            await _dbContext.ChiTietLichSuThis.AddAsync(chiTiet);
                        }

                        // ✅ Upsert câu sai: tăng SoLanSai nếu đã tồn tại
                        var userId = lichSuThi.UserId ?? string.Empty;

                        var wrongQuestionIds = chiTietList
                            .Where(ct => ct.DungSai == true)   // giữ theo code gốc của chị
                            .Select(ct => ct.CauHoiId)
                            .Distinct()
                            .ToList();

                        if (wrongQuestionIds.Count > 0 && !string.IsNullOrEmpty(userId))
                        {
                            var existing = await _dbContext.CauHoiSais
                                .Where(x => x.UserId == userId && wrongQuestionIds.Contains(x.CauHoiId))
                                .ToListAsync();

                            foreach (var qid in wrongQuestionIds)
                            {
                                var row = existing.FirstOrDefault(x => x.CauHoiId == qid);
                                if (row != null)
                                {
                                    row.SoLanSai += 1;
                                    row.NgaySai = DateTime.Now;
                                }
                                else
                                {
                                    await _dbContext.CauHoiSais.AddAsync(new CauHoiSai
                                    {
                                        UserId = userId,
                                        CauHoiId = qid,
                                        NgaySai = DateTime.Now,
                                        SoLanSai = 1
                                    });
                                }
                            }
                        }

                        await _dbContext.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return true;
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving exam history: {ex.Message}");
                return false;
            }
        }


        public async Task<List<CauHoi>> GetCauHoiSaiByUserAsync(string userId)
        {
            return await _dbContext.CauHoiSais
                .AsNoTracking()
                .Where(c => c.UserId == userId)
                .Include(c => c.CauHoi)
                    .ThenInclude(ch => ch.ChuDe)
                .Select(c => c.CauHoi)
                .Distinct()
                .ToListAsync();
        }

        public async Task<CauHoi?> GetCauHoiByIdAsync(Guid id)
        {
            return await _dbContext.CauHois.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<CauHoiSai>> GetCauHoiSaiListAsync(string userId, Guid cauHoiId)
        {
            return await _dbContext.CauHoiSais
                .Where(c => c.UserId == userId && c.CauHoiId == cauHoiId)
                .ToListAsync();
        }

        public Task XoaCauHoiSaisAsync(List<CauHoiSai> list)
        {
            _dbContext.CauHoiSais.RemoveRange(list);
            return Task.CompletedTask;
        }


        public async Task AddCauHoiSaiAsync(CauHoiSai cauHoiSai)
        {
            await _dbContext.CauHoiSais.AddAsync(cauHoiSai);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public ApplicationDbContext GetDbContext()
        {
            return _dbContext;
        }
    }
}

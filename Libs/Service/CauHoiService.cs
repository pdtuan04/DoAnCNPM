using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libs.Entity;
using Libs.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Libs.Service
{
    public class CauHoiService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICauHoiRepository _cauHoiRepository;
        private readonly IChuDeRepository _chuDeRepository;
        public CauHoiService(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
            this._cauHoiRepository= new CauHoiRepository(dbContext);
            this._chuDeRepository = new ChuDeRepository(dbContext);
        }
        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<CauHoi>> GetAllCauHoiAsync()
        {
            return await _cauHoiRepository.GetAllCauHoiAsync();
        }

        public async Task<CauHoi> GetCauHoiByIdAsync(Guid id)
        {
            return await _cauHoiRepository.GetCauHoiByIdAsync(id);
        }

        public async Task<PageList<CauHoi>> GetPagedCauHoi(int pageNumber, int pageSize, string? search, string? sortCol, string sortDir)
        {
            return await _cauHoiRepository.GetPagedCauHoi(pageNumber, pageSize, search, sortCol, sortDir);
        }

        public async Task<CauHoi> CreateCauHoiAsync(CauHoi cauHoi)
        {
            if (cauHoi == null)
            {
                throw new ArgumentNullException(nameof(cauHoi));
            }

            if (cauHoi.Id == Guid.Empty)
            {
                cauHoi.Id = Guid.NewGuid();
            }

            _cauHoiRepository.Add(cauHoi);
            await _dbContext.SaveChangesAsync();
            return cauHoi;
        }

        public async Task<CauHoi> UpdateCauHoiAsync(CauHoi cauHoi)
        {
            if (cauHoi == null)
            {
                throw new ArgumentNullException(nameof(cauHoi));
            }

            var existingCauHoi = await _cauHoiRepository.GetCauHoiByIdAsync(cauHoi.Id);
            if (existingCauHoi == null)
            {
                return null;
            }

            // Update properties
            existingCauHoi.NoiDung = cauHoi.NoiDung;
            existingCauHoi.LuaChonA = cauHoi.LuaChonA;
            existingCauHoi.LuaChonB = cauHoi.LuaChonB;
            existingCauHoi.LuaChonC = cauHoi.LuaChonC;
            existingCauHoi.LuaChonD = cauHoi.LuaChonD;
            existingCauHoi.DapAnDung = cauHoi.DapAnDung;
            existingCauHoi.GiaiThich = cauHoi.GiaiThich;
            existingCauHoi.DiemLiet = cauHoi.DiemLiet;
            existingCauHoi.MediaUrl = cauHoi.MediaUrl;
            existingCauHoi.LoaiMedia = cauHoi.LoaiMedia;
            existingCauHoi.MeoGhiNho = cauHoi.MeoGhiNho;
            existingCauHoi.ChuDeId = cauHoi.ChuDeId;
            existingCauHoi.LoaiBangLaiId = cauHoi.LoaiBangLaiId;

            _cauHoiRepository.Update(existingCauHoi);
            await _dbContext.SaveChangesAsync();
            return existingCauHoi;
        }

        public async Task<bool> DeleteCauHoiAsync(Guid id)
        {
            var cauHoi = await _cauHoiRepository.GetCauHoiByIdAsync(id);
            if (cauHoi == null)
            {
                return false;
            }

            cauHoi.isDeleted = true;
            _cauHoiRepository.Update(cauHoi);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<List<CauHoi>> CauHoiHaySai(int soLuong)
        {
            var cauHoi = await _cauHoiRepository.CauHoiHaySai(soLuong);
            return cauHoi;
        }

        public async Task<PageList<CauHoi>> GetCauHoisOnTapTheoChuDeAsync(int pageNumber, int pageSize, string? search, string? sortCol, string sortDir, Guid? chuDeId, Guid? LoaiBangLaiId)
        {
            var cauHoi = await _cauHoiRepository.GetCauHoisOnTapTheoChuDeAsync(pageNumber, pageSize, search, sortCol, sortDir, chuDeId, LoaiBangLaiId);
            return cauHoi;
        }

        public async Task<List<CauHoi>> GetCauHoiNgauNhien(int soLuong)
        {
            var cauHoi = await _cauHoiRepository.GetCauHoiNgauNhien(soLuong);
            return cauHoi;
        }
        public async Task<List<CauHoi>> GetCauHoiNgauNhienTheoChuDe(Guid? loaiBangLaiId, Guid chuDeId, int soLuong)
        {
            var cauHoi = await _cauHoiRepository.GetCauHoiNgauNhienTheoChuDe(loaiBangLaiId, chuDeId, soLuong);

            if (cauHoi.Count < soLuong)
            {
                var tenChuDe = await _chuDeRepository.GetTenChuDeByIdAsync(chuDeId);
                throw new InvalidOperationException($"Không đủ câu hỏi trong chủ đề {tenChuDe}.");
            }
            return cauHoi;
        }
    }
}

using Libs.Entity;
using Libs.Models;
using Libs.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Libs.Service
{
    public class LichSuThiService
    {
        private readonly ILichSuThiRepository _lichSuThiRepository;

        public LichSuThiService(ILichSuThiRepository lichSuThiRepository)
        {
            _lichSuThiRepository = lichSuThiRepository;
        }

        public async Task<PageList<LichSuThi>> GetLichSuThiByUser(string userId, int pageNumber = 1, int pageSize = 10)
        {
            return await _lichSuThiRepository.GetLichSuThiByUserAsync(userId, pageNumber, pageSize);
        }

        public async Task<LichSuThiDetailModel> GetLichSuThiDetail(Guid lichSuThiId)
        {
            return await _lichSuThiRepository.GetLichSuThiDetailAsync(lichSuThiId);
        }

        public async Task<LichSuThiStatModel> GetLichSuThiStats(string userId)
        {
            return await _lichSuThiRepository.GetLichSuThiStatsAsync(userId);
        }

        public async Task<List<CauHoiSaiFrequencyModel>> GetFrequentWrongQuestions(string userId, int limit = 10)
        {
            return await _lichSuThiRepository.GetFrequentWrongQuestionsAsync(userId, limit);
        }

        public async Task<bool> DeleteLichSuThi(Guid lichSuThiId, string userId)
        {
            try
            {
                return await _lichSuThiRepository.DeleteLichSuThiAsync(lichSuThiId, userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LichSuThiService.DeleteLichSuThi: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SaveLichSuThi(LichSuThi lichSuThi, List<ChiTietLichSuThi> chiTietList)
        {
            try
            {
                if (lichSuThi == null)
                    throw new ArgumentNullException(nameof(lichSuThi));

                if (chiTietList == null || !chiTietList.Any())
                    throw new ArgumentException("Chi tiết lịch sử thi không được rỗng");

                return await _lichSuThiRepository.SaveLichSuThiAsync(lichSuThi, chiTietList);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LichSuThiService.SaveLichSuThi: {ex.Message}");
                return false;
            }
        }
    }
}
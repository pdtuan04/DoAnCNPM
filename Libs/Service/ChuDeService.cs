using Libs.Entity;
using Libs.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Libs.Service
{
    public class ChuDeService
    {
        private readonly IChuDeRepository _chuDeRepository;

        public ChuDeService(IChuDeRepository chuDeRepository)
        {
            _chuDeRepository = chuDeRepository;
        }

        public async Task<List<ChuDe>> GetDanhSachChuDe()
        {
            return await _chuDeRepository.GetDanhSachChuDeAsync();
        }

        public async Task<List<CauHoi>> GetCauHoiTheoChuDe(Guid loaiBangLaiId, Guid chuDeId)
        {
            return await _chuDeRepository.GetCauHoiTheoChuDeAsync(loaiBangLaiId, chuDeId);
        }

        public async Task<string> GetTenChuDeById(Guid chuDeId)
        {
            return await _chuDeRepository.GetTenChuDeByIdAsync(chuDeId);
        }
    }
}

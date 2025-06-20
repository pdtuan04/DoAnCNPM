using Libs.Entity;
using Libs.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Libs.Service
{
    public class LoaiBangLaiService
    {
        private readonly ILoaiBangLaiRepository _loaiBangLaiRepository;

        public LoaiBangLaiService(ILoaiBangLaiRepository loaiBangLaiRepository)
        {
            _loaiBangLaiRepository = loaiBangLaiRepository;
        }

        public async Task<List<LoaiBangLai>> GetXeMayAsync()
        {
            return await _loaiBangLaiRepository.GetLoaiBangLaiXeMayAsync();
        }

        public async Task<List<LoaiBangLai>> GetOToAsync()
        {
            return await _loaiBangLaiRepository.GetLoaiBangLaiOToAsync();
        }

        public async Task<List<LoaiBangLai>> GetDanhSachLoaiBangLaiAsync()
        {
            return await _loaiBangLaiRepository.GetDanhSachLoaiBangLaiAsync();
        }

        public async Task<LoaiBangLai> GetByIdAsync(Guid id)
        {
            return await _loaiBangLaiRepository.GetLoaiBangLaiByIdAsync(id);
        }

        public async Task<(LoaiBangLai, List<ChuDe>)> GetChuDeByLoaiBangLaiAsync(Guid id)
        {
            return await _loaiBangLaiRepository.GetChuDeByLoaiBangLaiAsync(id);
        }

        
    }
}

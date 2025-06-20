using Libs.Entity;
using Libs.Models;
using Libs.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Libs.Service
{
    public class BaiThiService
    {
        private readonly IBaiThiRepository _baiThiRepository;

        public BaiThiService(IBaiThiRepository baiThiRepository)
        {
            _baiThiRepository = baiThiRepository;
        }

        public async Task<BaiThi> GetBaiThiWithDetails(Guid id)
        {
            return await _baiThiRepository.GetBaiThiWithDetails(id);
        }

        public (List<KetQuaBaiThi> ketQuaList, float diem, int tongSoCau, int diemToiThieu)
            ChamDiem(BaiThi baiThi, Dictionary<Guid, string> answers)
        {
            return _baiThiRepository.ChamDiem(baiThi, answers);
        }

        public async Task<NopBaiThiResult> XuLyNopBaiThi(SubmitBaiThiRequest request, string userId)
        {
            return await _baiThiRepository.XuLyNopBaiThi(request, userId);
        }

        public async Task<BaiThi> GetDeThiNgauNhien(Guid loaiBangLaiId)
        {
            return await _baiThiRepository.GetDeThiNgauNhien(loaiBangLaiId);
        }

        public async Task<BaiThi> GetChiTietBaiThi(Guid id)
        {
            return await _baiThiRepository.GetChiTietBaiThi(id);
        }

        public async Task<List<BaiThi>> GetDanhSachBaiThi()
        {
            return await _baiThiRepository.GetDanhSachBaiThi();
        }

        public async Task<List<BaiThi>> GetDanhSachDeThi(string loaiXe = null)
        {
            return await _baiThiRepository.GetDanhSachDeThi(loaiXe);
        }

        public async Task<List<BaiThi>> GetDeThiByLoaiBangLai(Guid loaiBangLaiId)
        {
            return await _baiThiRepository.GetDeThiByLoaiBangLai(loaiBangLaiId);
        }

        public async Task<List<CauHoi>> GetCauHoiOnTap(Guid loaiBangLaiId)
        {
            return await _baiThiRepository.GetCauHoiOnTap(loaiBangLaiId);
        }
    }
}

using Libs.Models;
using Libs.Repositories;
using Libs.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ET.Controllers
{
    public class BaiThiController : Controller
    {
        private readonly BaiThiService _baiThiService;

        private readonly  LoaiBangLaiService _loaiBangLaiService;
        public BaiThiController(BaiThiService baiThiService, LoaiBangLaiService loaiBangLaiService)
        {
            _baiThiService = baiThiService;
            _loaiBangLaiService= loaiBangLaiService;
        }

        public async Task<IActionResult> LamBaiThi(Guid id, Dictionary<Guid, string> answers)
        {
            var baiThi = await _baiThiService.GetBaiThiWithDetails(id);
            if (baiThi == null)
                return NotFound();

            var (ketQuaList, diem, tongSoCau, diemToiThieu) = _baiThiService.ChamDiem(baiThi, answers);

            ViewBag.KetQuaList = ketQuaList;
            ViewBag.TongSoCau = tongSoCau;
            ViewBag.DiemToiThieu = diemToiThieu;
            ViewBag.Diem = diem;

            return View(baiThi);
        }

        public IActionResult ChonDeThi()
        {
            // Nếu bạn có service cho lấy chủ đề và loại bằng lái, có thể thêm vào BaiThiService
            return View();
        }

        public async Task<IActionResult> ChiTietBaiThi(Guid id)
        {
            var baiThi = await _baiThiService.GetChiTietBaiThi(id);
            if (baiThi == null) return NotFound();
            return View(baiThi);
        }

        public async Task<IActionResult> DanhSachBaiThi()
        {
            var list = await _baiThiService.GetDanhSachBaiThi();
            return View(list);
        }

        public async Task<IActionResult> DanhSachDeThi(string loaiXe)
        {
            var list = await _baiThiService.GetDanhSachDeThi(loaiXe);
            return View(list);
        }

        public IActionResult LoaiBangLaiXeMay()
        {
            // Nếu có service cho phần này thì tách ra, tạm thời không dùng trong service
            return View();
        }

        public IActionResult LoaiBangLaiOTo()
        {
            return View();
        }

        public async Task<IActionResult> ThiDe(Guid loaiBangLaiId)
        {
            // Giả sử GetDeThiByLoaiBangLai có trong service
            var list = await _baiThiService.GetDeThiByLoaiBangLai(loaiBangLaiId);
            return View(list);
        }

        public async Task<IActionResult> LamDeNgauNhien(Guid loaiBangLaiId)
        {
            var deThi = await _baiThiService.GetDeThiNgauNhien(loaiBangLaiId);
            if (deThi == null) return NotFound("Không có đề thi nào phù hợp.");
            return RedirectToAction("LamBaiThi", new { id = deThi.Id });
        }

        public async Task<IActionResult> OnTap(Guid loaiBangLaiId)
        {
            if (loaiBangLaiId == Guid.Empty)
                return BadRequest("Thiếu ID loại bằng lái");

            var loai = await _loaiBangLaiService.GetByIdAsync(loaiBangLaiId);
            if (loai == null)
                return NotFound("Không tìm thấy loại bằng lái");

            ViewBag.LoaiBangLaiId = loai.Id;
            ViewBag.TenLoai = loai.TenLoai;

            return View(); // Không cần truyền model vì view dùng fetch
        }



        public IActionResult OnTapChuDe(Guid loaiBangLaiId, int chuDeId)
        {
            // Chức năng này chưa có trong service, nếu cần thì bổ sung thêm vào service
            return View();
        }

        public IActionResult ChonChuDe(Guid loaiBangLaiId)
        {
            // Cũng chưa có trong service
            return View();
        }

        public IActionResult DanhSachLoaiBangLai()
        {
            return View();
        }
    }
}
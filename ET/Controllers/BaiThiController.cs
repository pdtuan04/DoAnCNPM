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
           

            return View();
        }

        public IActionResult ChonDeThi()
        {
            return View();
        }

        public async Task<IActionResult> ChiTietBaiThi(Guid id)
        {
          
            return View();
        }

       
        public async Task<IActionResult> DanhSachDeThi(Guid loaiBangLaiId)
        {
            return View();
        }

        public async Task<IActionResult> LamDeNgauNhien(Guid loaiBangLaiId)
        {
            var deThi = await _baiThiService.GetDeThiNgauNhien(loaiBangLaiId);
            if (deThi == null) return NotFound("Không có đề thi nào phù hợp.");
            return RedirectToAction("LamBaiThi", new { id = deThi.Id });
        }

        public async Task<IActionResult> OnTap(Guid loaiBangLaiId)
        {        
            return View(); 
        }
        public IActionResult OnTapChuDe(Guid loaiBangLaiId, int chuDeId)
        {
            return View();
        }

        public IActionResult ChonChuDe(Guid loaiBangLaiId)
        {
            return View();
        }

        
    }
}
using Libs.Service;
using Microsoft.AspNetCore.Mvc;

namespace ET.Controllers
{
    public class ChuDeController : Controller
    {
        private readonly ChuDeService _chuDeService;
        private readonly LoaiBangLaiService _loaiBangLaiService;

        public ChuDeController(ChuDeService chuDeService, LoaiBangLaiService loaiBangLaiService)
        {
            _chuDeService = chuDeService;
            _loaiBangLaiService = loaiBangLaiService;
        }

        public async Task<IActionResult> ChonChuDe(Guid loaiBangLaiId)
        {         
            return View();
        }

        public async Task<IActionResult> OnTapChuDe(Guid loaiBangLaiId, Guid chuDeId)
        {
            var list = await _chuDeService.GetCauHoiTheoChuDe(loaiBangLaiId, chuDeId);
            if (list.Count == 0)
            {
                TempData["Error"] = "Không có câu hỏi nào cho chủ đề này.";
                return RedirectToAction("OnTap", "BaiThiView", new { loaiBangLaiId });
            }

            ViewBag.LoaiBangLai = await _loaiBangLaiService.GetByIdAsync(loaiBangLaiId);
            ViewBag.TenChuDe = await _chuDeService.GetTenChuDeById(chuDeId);

            return View("OnTapChuDe", list);
        }
    }
}

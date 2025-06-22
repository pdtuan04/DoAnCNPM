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
            return View();
        }
    }
}

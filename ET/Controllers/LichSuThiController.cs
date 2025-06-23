using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ET.Controllers
{
    [Authorize]
    public class LichSuThi1Controller : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail(Guid id)
        {
            ViewData["ExamId"] = id;
            return View();
        }
        public IActionResult LuyenLaiCauSai()
        {
            return View();
        }
        public IActionResult KetQuaLuyLai()
        {
            return View();
        }

    }
}
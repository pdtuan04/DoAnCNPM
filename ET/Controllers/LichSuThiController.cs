using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ET.Controllers
{
    [Authorize]
    public class LichSuThiController : Controller
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
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ET.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult QLMoPhong()
        {
            return View();
        }
        public IActionResult CreateMoPhong()
        {
            return View();
        }

        public IActionResult EditMoPhong(Guid id)
        {
            return View(id);
        }

        public IActionResult DetailsMoPhong(Guid id)
        {
            return View(id);
        }
    }
}

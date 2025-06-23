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
        public IActionResult QLCauHoi()
        {
            return View();
        }
        public IActionResult CreateCauHoi()
        {
            return View();
        }
        public IActionResult EditCauHoi(Guid id)
        {
            return View(id);
        }
        public IActionResult DetailsCauHoi(Guid id)
        {
            return View(id);
        }
        public IActionResult QLbaocaochiase()
        {
            return View();
        }
        public IActionResult QLLoaiBangLai()
        {
            return View();
        }

        public IActionResult CreateLoaiBangLai()
        {
            return View();
        }

        public IActionResult EditLoaiBangLai(Guid id)
        {
            return View(id);
        }

        public IActionResult DetailsLoaiBangLai(Guid id)
        {
            return View(id);
        }
    }
}

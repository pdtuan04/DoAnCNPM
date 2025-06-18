using Microsoft.AspNetCore.Mvc;

namespace ET.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

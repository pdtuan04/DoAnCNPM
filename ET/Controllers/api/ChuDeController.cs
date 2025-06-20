using Microsoft.AspNetCore.Mvc;
using Libs.Service;
using System;
using System.Threading.Tasks;

namespace YourProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChuDeController : ControllerBase
    {
        private readonly ChuDeService _chuDeService;

        public ChuDeController(ChuDeService chuDeService)
        {
            _chuDeService = chuDeService;
        }

        [HttpGet("danh-sach")]
        public async Task<IActionResult> GetDanhSach()
        {
            var list = await _chuDeService.GetDanhSachChuDe();
            return Ok(list);
        }

        [HttpGet("cau-hoi")]
        public async Task<IActionResult> GetCauHoiTheoChuDe(Guid loaiBangLaiId, Guid chuDeId)
        {
            var list = await _chuDeService.GetCauHoiTheoChuDe(loaiBangLaiId, chuDeId);
            return Ok(list);
        }

        [HttpGet("ten/{chuDeId}")]
        public async Task<IActionResult> GetTenChuDe(Guid chuDeId)
        {
            var ten = await _chuDeService.GetTenChuDeById(chuDeId);
            return Ok(new { tenChuDe = ten });
        }
    }
}
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
        private readonly LoaiBangLaiService _loaiBangLaiService;


        public ChuDeController(ChuDeService chuDeService, LoaiBangLaiService loaiBangLaiService)
        {
            _chuDeService = chuDeService;
            _loaiBangLaiService = loaiBangLaiService;
        }

        [HttpGet("danh-sach")]
        public async Task<IActionResult> GetDanhSach()
        {
            var list = await _chuDeService.GetDanhSachChuDe();
            return Ok(list);
        }

        
        [HttpGet("ten/{chuDeId}")]
        public async Task<IActionResult> GetTenChuDe(Guid chuDeId)
        {
            var ten = await _chuDeService.GetTenChuDeById(chuDeId);
            return Ok(new { tenChuDe = ten });
        }
        [HttpGet("{id}/chu-de")]
        public async Task<IActionResult> GetChuDeByLoaiBangLai(Guid id)
        {
            var (loai, chuDeList) = await _loaiBangLaiService.GetChuDeByLoaiBangLaiAsync(id);

            var result = new
            {
                loai = new
                {
                    loai.Id,
                    loai.TenLoai
                },
                chuDeList = chuDeList.Select(cd => new
                {
                    cd.Id,
                    cd.TenChuDe,
                    cd.ImageUrl,
                    SoCau = cd.CauHois?.Count ?? 0
                })
            };

            return Ok(result);
        }
    }
}
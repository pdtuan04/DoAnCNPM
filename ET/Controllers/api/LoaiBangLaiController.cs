using Microsoft.AspNetCore.Mvc;
using Libs.Service;
using Libs.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YourProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoaiBangLaiController : ControllerBase
    {
        private readonly LoaiBangLaiService _loaiBangLaiService;

        public LoaiBangLaiController(LoaiBangLaiService loaiBangLaiService)
        {
            _loaiBangLaiService = loaiBangLaiService;
        }

        [HttpGet("danh-sach")]
        public async Task<IActionResult> GetDanhSach()
        {
            var list = await _loaiBangLaiService.GetDanhSachLoaiBangLaiAsync();
            return Ok(list);
        }

        [HttpGet("xe-may")]
        public async Task<IActionResult> GetXeMay()
        {
            var list = await _loaiBangLaiService.GetXeMayAsync();
            return Ok(list);
        }

        [HttpGet("oto")]
        public async Task<IActionResult> GetOTo()
        {
            var list = await _loaiBangLaiService.GetOToAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var loai = await _loaiBangLaiService.GetByIdAsync(id);
            return loai == null ? NotFound() : Ok(loai);
        }

       
        [HttpGet("get-loai-bang-lai-list")]
        public IActionResult GetLoaiBangLaiList()
        {
            List<LoaiBangLai> loaiBangLaiList = _loaiBangLaiService.GetAllLoaiBangLais();
            return Ok(new { status = true, message = "Get Loai Bang Lai Successfully", data = loaiBangLaiList });
        }
    }
}
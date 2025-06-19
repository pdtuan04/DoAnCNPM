using ET.Models;
using Libs.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ET.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoPhongController : ControllerBase
    {
        private MoPhongService moPhongService;
        public MoPhongController(MoPhongService moPhongService)
        {
            this.moPhongService = moPhongService;
        }
        [HttpGet("get-mo-phong-by-loai-bang-lai")]
        public IActionResult GetMoPhong(Guid id)
        {
            var result = moPhongService.GetMoPhongByLoaiBangLaiId(id);

            if (result == null || !result.Any())
            {
                return Ok(new { status = false, message = "Mô phỏng không tìm thấy" });
            }
            IEnumerable<MoPhongModel> moPhongModels = result.Select(x => new MoPhongModel
            {
                NoiDung = x.NoiDung,
                VideoUrl = x.VideoUrl,
                DapAn = x.DapAn,
            }).ToList();
            return Ok(new { status = true, message = "Lấy mô phỏng thành công", data = moPhongModels });
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("get-all-mo-phong")]
        public IActionResult GetAllMoPhong()
        {
            var result = moPhongService.GetAllMoPhong();
            if (result == null || !result.Any())
            {
                return Ok(new { status = false, message = "Không có mô phỏng nào" });
            }
            return Ok(new { status = true, message = "Lấy tất cả mô phỏng thành công", data = result });
        }
        [HttpGet("{id}")]
        public IActionResult GetMoPhongById(Guid id)
        {
            var result = moPhongService.GetMoPhongById(id);
            if (result == null)
            {
                return Ok(new { status = false, message = "Mô phỏng không tìm thấy" });
            }
            MoPhongModel moPhongModel = new MoPhongModel
            {
                NoiDung = result.NoiDung,
                VideoUrl = result.VideoUrl,
                DapAn = result.DapAn,
            };
            return Ok(new { status = true, message = "Lấy mô phỏng thành công", data = moPhongModel });
        }
    }
}

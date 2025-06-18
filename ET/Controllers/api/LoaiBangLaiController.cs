using Libs.Entity;
using Libs.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ET.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoaiBangLaiController : ControllerBase
    {
        private LoaiBangLaiService _loaiBangLaiService;
        public LoaiBangLaiController(LoaiBangLaiService loaiBangLaiService)
        {
            this._loaiBangLaiService = loaiBangLaiService;
        }

        [HttpGet("get-loai-bang-lai-list")]
        public IActionResult GetLoaiBangLaiList()
        {
            List<LoaiBangLais> loaiBangLaiList = _loaiBangLaiService.GetAllLoaiBangLais();
            return Ok(new { status = true, message = "Get Loai Bang Lai Successfully", data = loaiBangLaiList });
        }

        [HttpGet("get-loai-bang-lai-by-id/{id}")]
        public IActionResult GetLoaiBangLaiById(Guid id)
        {
            LoaiBangLais loaiBangLai = _loaiBangLaiService.GetLoaiBangLaiById(id);
            if (loaiBangLai == null)
            {
                return NotFound(new { status = false, message = "Loai Bang Lai not found" });
            }
            return Ok(new { status = true, message = "Get Loai Bang Lai Successfully", data = loaiBangLai });
        }

        [HttpGet("get-loai-bang-lai-by-name/{name}")]
        public IActionResult GetLoaiBangLaiByName(string name)
        {
            LoaiBangLais loaiBangLai = _loaiBangLaiService.GetLoaiBangLaiByName(name);
            if (loaiBangLai == null)
            {
                return NotFound(new { status = false, message = "Loai Bang Lai not found" });
            }
            return Ok(new { status = true, message = "Get Loai Bang Lai Successfully", data = loaiBangLai });
        }

        [HttpGet("get-loai-bang-lai-by-loai-xe/{loaiXe}")]
        public IActionResult GetLoaiBangLaiByLoaiXe(string loaiXe)
        {
            LoaiBangLais loaiBangLai = _loaiBangLaiService.GetLoaiBangLaiByLoaiXe(loaiXe);
            if (loaiBangLai == null)
            {
                return NotFound(new { status = false, message = "Loai Bang Lai not found" });
            }
            return Ok(new { status = true, message = "Get Loai Bang Lai Successfully", data = loaiBangLai });
        }

        [HttpGet("get-loai-bang-lai-with-questions")]
        public IActionResult GetLoaiBangLaiWithQuestions()
        {
            List<LoaiBangLais> loaiBangLais = _loaiBangLaiService.GetLoaiBangLaisWithQuestions();
            return Ok(new { status = true, message = "Get Loai Bang Lai With Questions Successfully", data = loaiBangLais });
        }

        [HttpGet("get-loai-bang-lai-with-questions/{id}")]
        public IActionResult GetLoaiBangLaiWithQuestions(Guid id)
        {
            LoaiBangLais loaiBangLai = _loaiBangLaiService.GetLoaiBangLaiWithQuestions(id);
            if (loaiBangLai == null)
            {
                return NotFound(new { status = false, message = "Loai Bang Lai not found" });
            }
            return Ok(new { status = true, message = "Get Loai Bang Lai With Questions Successfully", data = loaiBangLai });
        }

        [HttpPost("create-loai-bang-lai")]
        public IActionResult CreateLoaiBangLai(LoaiBangLais loaiBangLai)
        {
            _loaiBangLaiService.CreateLoaiBangLai(loaiBangLai);
            return Ok(new { status = true, message = "Create Loai Bang Lai Successfully" });
        }

        [HttpPut("update-loai-bang-lai")]
        public IActionResult UpdateLoaiBangLai(LoaiBangLais loaiBangLai)
        {
            if (!_loaiBangLaiService.IsLoaiBangLaiExists(loaiBangLai.Id))
            {
                return NotFound(new { status = false, message = "Loai Bang Lai not found" });
            }
            _loaiBangLaiService.UpdateLoaiBangLai(loaiBangLai);
            return Ok(new { status = true, message = "Update Loai Bang Lai Successfully" });
        }

        [HttpDelete("delete-loai-bang-lai/{id}")]
        public IActionResult DeleteLoaiBangLai(Guid id)
        {
            if (!_loaiBangLaiService.IsLoaiBangLaiExists(id))
            {
                return NotFound(new { status = false, message = "Loai Bang Lai not found" });
            }
            _loaiBangLaiService.DeleteLoaiBangLai(id);
            return Ok(new { status = true, message = "Delete Loai Bang Lai Successfully" });
        }
    }
}
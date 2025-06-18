using Libs.Entity;
using Libs.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ET.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChuDeController : ControllerBase
    {
        private ChuDeService _chuDeService;
        public ChuDeController(ChuDeService chuDeService)
        {
            this._chuDeService = chuDeService;
        }

        [HttpGet("get-chu-de-list")]
        public IActionResult GetChuDeList()
        {
            List<ChuDes> chuDeList = _chuDeService.GetAllChuDes();
            return Ok(new { status = true, message = "Get Chu De Successfully", data = chuDeList });
        }

        [HttpGet("get-chu-de-by-id/{id}")]
        public IActionResult GetChuDeById(Guid id)
        {
            ChuDes chuDe = _chuDeService.GetChuDeById(id);
            if (chuDe == null)
            {
                return NotFound(new { status = false, message = "Chu De not found" });
            }
            return Ok(new { status = true, message = "Get Chu De Successfully", data = chuDe });
        }

        [HttpGet("get-chu-de-with-questions")]
        public IActionResult GetChuDeWithQuestions()
        {
            List<ChuDes> chuDes = _chuDeService.GetChuDesWithQuestions();
            return Ok(new { status = true, message = "Get Chu De With Questions Successfully", data = chuDes });
        }

        [HttpGet("get-chu-de-with-questions/{id}")]
        public IActionResult GetChuDeWithQuestions(Guid id)
        {
            ChuDes chuDe = _chuDeService.GetChuDeWithQuestions(id);
            if (chuDe == null)
            {
                return NotFound(new { status = false, message = "Chu De not found" });
            }
            return Ok(new { status = true, message = "Get Chu De With Questions Successfully", data = chuDe });
        }

        [HttpPost("create-chu-de")]
        public IActionResult CreateChuDe(ChuDes chuDe)
        {
            _chuDeService.CreateChuDe(chuDe);
            return Ok(new { status = true, message = "Create Chu De Successfully" });
        }

        [HttpPut("update-chu-de")]
        public IActionResult UpdateChuDe(ChuDes chuDe)
        {
            if (!_chuDeService.IsChuDeExists(chuDe.Id))
            {
                return NotFound(new { status = false, message = "Chu De not found" });
            }
            _chuDeService.UpdateChuDe(chuDe);
            return Ok(new { status = true, message = "Update Chu De Successfully" });
        }

        [HttpDelete("delete-chu-de/{id}")]
        public IActionResult DeleteChuDe(Guid id)
        {
            if (!_chuDeService.IsChuDeExists(id))
            {
                return NotFound(new { status = false, message = "Chu De not found" });
            }
            _chuDeService.DeleteChuDe(id);
            return Ok(new { status = true, message = "Delete Chu De Successfully" });
        }
    }
}
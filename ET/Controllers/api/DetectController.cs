using Libs.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ET.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetectController : ControllerBase
    {
        private readonly YoloService _yoloService;

        public DetectController(YoloService yoloService)
        {
            _yoloService = yoloService;
        }

        [HttpPost("detect-sign")]
        public IActionResult Post(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Vui lòng upload ảnh.");

            try
            {
                using var stream = file.OpenReadStream();
                var results = _yoloService.Detect(stream);

                return Ok(new
                {
                    Message = "Phát hiện thành công",
                    Count = results.Count,
                    Data = results
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi server: " + ex.Message);
            }
        }
        [HttpGet("test")]
        public IActionResult Test()
        {
            try
            {
                // Đường dẫn tuyệt đối hoặc tương đối tới file ảnh
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/img_0027.jpg");

                // Mở stream từ file ảnh
                using var stream = System.IO.File.OpenRead(filePath);

                // Gọi YoloService
                var results = _yoloService.Detect(stream);

                return Ok(new
                {
                    Message = "Phát hiện thành công",
                    Count = results.Count,
                    Data = results
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi server: " + ex.Message);
            }
        }

    }
}

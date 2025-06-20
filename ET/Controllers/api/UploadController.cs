﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ET.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        [HttpPost("upload-video")]
        public async Task<IActionResult> UploadVideo(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { status = false, message = "Không có file nào được chọn" });
                }

                // Kiểm tra định dạng file
                string[] allowedExtensions = { ".mp4", ".webm", ".ogg", ".mov" };
                string fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new { status = false, message = "Chỉ chấp nhận file video định dạng MP4, WebM, OGG, MOV" });
                }

                // Tạo tên file duy nhất để tránh trùng lặp
                string uniqueFileName = Guid.NewGuid().ToString() + fileExtension;

                // Đường dẫn thư mục lưu file (tương đối với wwwroot)
                string uploadsFolder = Path.Combine("videos");

                // Đảm bảo thư mục tồn tại
                string physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", uploadsFolder);
                if (!Directory.Exists(physicalPath))
                {
                    Directory.CreateDirectory(physicalPath);
                }

                // Đường dẫn đầy đủ đến file
                string filePath = Path.Combine(physicalPath, uniqueFileName);

                // Lưu file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Trả về đường dẫn tương đối để lưu trong database
                string relativePath = $"/{uploadsFolder.Replace('\\', '/')}/{uniqueFileName}";

                return Ok(new { status = true, message = "Upload video thành công", filePath = relativePath });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = false, message = $"Lỗi khi upload file: {ex.Message}" });
            }
        }
    }
}

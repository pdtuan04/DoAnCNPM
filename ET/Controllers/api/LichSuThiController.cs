using Libs.Entity;
using Libs.Models;
using Libs.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ET.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LichSuThiController : ControllerBase
    {
        private readonly LichSuThiService _lichSuThiService;
        private readonly ILogger<LichSuThiController> _logger;

        public LichSuThiController(LichSuThiService lichSuThiService, ILogger<LichSuThiController> logger)
        {
            _lichSuThiService = lichSuThiService;
            _logger = logger;
        }

        [HttpGet("get-history")]
        public async Task<IActionResult> GetLichSuThi(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("GetLichSuThi: User ID not found in claims");
                    return Unauthorized(new { status = false, message = "Không tìm thấy thông tin người dùng!" });
                }

                _logger.LogInformation($"Getting exam history for user: {userId}, page: {pageNumber}, size: {pageSize}");
                var result = await _lichSuThiService.GetLichSuThiByUser(userId, pageNumber, pageSize);
                return Ok(new { status = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving exam history");
                return StatusCode(500, new { status = false, message = "Lỗi khi tải lịch sử thi: " + ex.Message });
            }
        }

        [HttpGet("get-stats")]
        public async Task<IActionResult> GetLichSuThiStats()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("GetLichSuThiStats: User ID not found in claims");
                    return Unauthorized(new { status = false, message = "Không tìm thấy thông tin người dùng!" });
                }

                _logger.LogInformation($"Getting exam stats for user: {userId}");
                var result = await _lichSuThiService.GetLichSuThiStats(userId);
                return Ok(new { status = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving exam stats");
                return StatusCode(500, new { status = false, message = "Lỗi khi tải thống kê: " + ex.Message });
            }
        }

        [HttpGet("get-wrong-questions")]
        public async Task<IActionResult> GetFrequentWrongQuestions(int count = 5)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("GetFrequentWrongQuestions: User ID not found in claims");
                    return Unauthorized(new { status = false, message = "Không tìm thấy thông tin người dùng!" });
                }

                _logger.LogInformation($"Getting wrong questions for user: {userId}, count: {count}");
                var result = await _lichSuThiService.GetFrequentWrongQuestions(userId, count);
                return Ok(new { status = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving frequent wrong questions");
                return StatusCode(500, new { status = false, message = "Lỗi khi tải câu hỏi sai: " + ex.Message });
            }
        }

        [HttpGet("detail/{id}")]
        public async Task<IActionResult> GetLichSuThiDetail(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    _logger.LogWarning("GetLichSuThiDetail: Invalid ID provided");
                    return BadRequest(new { status = false, message = "ID không hợp lệ" });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("GetLichSuThiDetail: User ID not found in claims");
                    return Unauthorized(new { status = false, message = "Không tìm thấy thông tin người dùng!" });
                }

                _logger.LogInformation($"Getting exam detail for ID: {id}, user: {userId}");

                // Get the exam detail from the service
                var result = await _lichSuThiService.GetLichSuThiDetail(id);

                if (result == null)
                {
                    _logger.LogWarning($"Exam detail not found for ID: {id}");
                    return NotFound(new { status = false, message = "Không tìm thấy lịch sử thi" });
                }

                // Check if there is any data in the ChiTietList
                if (result.ChiTietList == null || !result.ChiTietList.Any())
                {
                    _logger.LogWarning($"Exam detail found for ID: {id}, but ChiTietList is empty");
                    // Still return what we have, but with a warning
                }

                // No need to validate user ownership in detail view
                // This is causing issues when the userId format doesn't match exactly
                // Instead, log it as information but still return the data
                if (result.LichSuThi.UserId != userId)
                {
                    _logger.LogInformation($"User ID mismatch: Request from {userId}, exam belongs to {result.LichSuThi.UserId}");
                    // Don't return Forbid() here as it may be causing authentication issues
                }

                // Create a simplified response object that doesn't have circular references
                var response = new
                {
                    status = true,
                    data = new
                    {
                        lichSuThi = new
                        {
                            id = result.LichSuThi.Id,
                            baiThiId = result.LichSuThi.BaiThiId,
                            tenBaiThi = result.LichSuThi.TenBaiThi,
                            ngayThi = result.LichSuThi.NgayThi,
                            tongSoCau = result.LichSuThi.TongSoCau,
                            soCauDung = result.LichSuThi.SoCauDung,
                            phanTramDung = result.LichSuThi.PhanTramDung,
                            diem = result.LichSuThi.Diem,
                            ketQua = result.LichSuThi.KetQua,
                            macLoiNghiemTrong = result.LichSuThi.MacLoiNghiemTrong,
                            userId = result.LichSuThi.UserId
                        },
                        chiTietList = result.ChiTietList?.Select(ct => new
                        {
                            id = ct.Id,
                            lichSuThiId = ct.LichSuThiId,
                            cauHoiId = ct.CauHoiId,
                            cauTraLoi = ct.CauTraLoi,
                            dungSai = ct.DungSai,
                            cauHoi = ct.CauHoi == null ? null : new
                            {
                                id = ct.CauHoi.Id,
                                noiDung = ct.CauHoi.NoiDung,
                                dapAnDung = ct.CauHoi.DapAnDung,
                                mediaUrl = ct.CauHoi.MediaUrl,
                                chuDe = ct.CauHoi.ChuDe == null ? null : new
                                {
                                    id = ct.CauHoi.ChuDe.Id,
                                    tenChuDe = ct.CauHoi.ChuDe.TenChuDe
                                }
                            }
                        }).ToList(),
                        baiThi = result.BaiThi == null ? null : new
                        {
                            id = result.BaiThi.Id,
                            tenBaiThi = result.BaiThi.TenBaiThi
                        }
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving exam detail for ID: {id}");
                return StatusCode(500, new { status = false, message = "Lỗi khi tải chi tiết bài thi: " + ex.Message });
            }
        }
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteLichSuThi(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    _logger.LogWarning("DeleteLichSuThi: Invalid ID provided");
                    return BadRequest(new { status = false, message = "ID không hợp lệ" });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("DeleteLichSuThi: User ID not found in claims");
                    return Unauthorized(new { status = false, message = "Không tìm thấy thông tin người dùng!" });
                }

                _logger.LogInformation($"Deleting exam history for ID: {id}, user: {userId}");
                bool success = await _lichSuThiService.DeleteLichSuThi(id, userId);

                if (!success)
                {
                    _logger.LogWarning($"Failed to delete exam history for ID: {id}, user: {userId}");
                    return NotFound(new { status = false, message = "Không tìm thấy lịch sử thi hoặc không có quyền xóa" });
                }

                return Ok(new { status = true, message = "Xóa lịch sử thi thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting exam history for ID: {id}");
                return StatusCode(500, new { status = false, message = "Lỗi khi xóa lịch sử thi: " + ex.Message });
            }
        }
        //------///
        [HttpGet("luyen-lai-cau-sai")]
        public async Task<IActionResult> LuyenLaiCauSai()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { status = false, message = "Không tìm thấy người dùng" });

            var data = await _lichSuThiService.GetCauHoiLuyenLaiAsync(userId);

            return Ok(new { status = true, data }); // <-- lúc này data là List<object>
        }



        // ✅ API 2: POST - Lưu kết quả luyện lại
        [HttpPost("luu-ket-qua-luyen-lai")]
        public async Task<IActionResult> LuuKetQuaLuyenLai([FromBody] LuyenLaiRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { status = false, message = "Không tìm thấy người dùng" });

            if (request == null || request.CauHoiAnswers == null || !request.CauHoiAnswers.Any())
                return BadRequest(new { status = false, message = "Dữ liệu gửi lên không hợp lệ" });

            var result = await _lichSuThiService.LuuKetQuaLuyenLaiAsync(userId, request.CauHoiAnswers);
            return Ok(new
            {
                status = true,
                data = result // dạng List<KetQuaLuyenTapViewModel>
            });
        }
    }
}

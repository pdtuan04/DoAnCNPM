using Libs.Models;
using Libs.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ET.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaiThiController : ControllerBase
    {
        private readonly BaiThiService _baiThiService;
        private readonly ChuDeService _chuDeService;
        private readonly LoaiBangLaiService _loaiBangLaiService;
        public BaiThiController(BaiThiService baiThiService, ChuDeService chuDeService, LoaiBangLaiService loaiBangLaiService)
        {
            _baiThiService = baiThiService;
            _chuDeService = chuDeService;
            _loaiBangLaiService = loaiBangLaiService;
        }

        [HttpPost("nop-bai-thi")]
        public async Task<IActionResult> NopBaiThi([FromBody] SubmitBaiThiRequest request)
        {
            if (request == null || request.BaiThiId == Guid.Empty || request.Answers == null)
            {
                return BadRequest(new { success = false, message = "Dữ liệu gửi lên không hợp lệ!" });
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _baiThiService.XuLyNopBaiThi(request, userId);

                if (!result.Success)
                {
                    return BadRequest(new { success = false, message = result.Message });
                }

                return Ok(new
                {
                    success = true,
                    baiThiId = result.BaiThiId,
                    ketQuaList = result.KetQuaList.Select(kq => new
                    {
                        kq.CauHoiId,
                        kq.CauTraLoi,
                        kq.DapAnDung,
                        kq.DungSai
                    }),
                    soCauDung = result.SoCauDung,
                    tongSoCau = result.TongSoCau,
                    tongDiem = result.TongDiem,
                    ketQua = result.KetQua,
                    macLoiNghiemTrong = result.MacLoiNghiemTrong,
                    soCauLoiNghiemTrong = result.SoCauLoiNghiemTrong
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi xử lý bài thi.", error = ex.Message });
            }
        }

        [HttpGet("bai-thi/{id}")]
        public async Task<IActionResult> GetBaiThi(Guid id)
        {
            var baiThi = await _baiThiService.GetBaiThiWithDetails(id);
            if (baiThi == null)
                return NotFound(new { success = false, message = "Không tìm thấy bài thi!" });

            var result = new
            {
                baiThi.Id,
                baiThi.TenBaiThi,
                ChiTietBaiThis = baiThi.ChiTietBaiThis.Select(ct => new
                {
                    ct.Id,
                    CauHoi = new
                    {
                        ct.CauHoi.Id,
                        ct.CauHoi.NoiDung,
                        ct.CauHoi.LuaChonA,
                        ct.CauHoi.LuaChonB,
                        ct.CauHoi.LuaChonC,
                        ct.CauHoi.LuaChonD,
                        ct.CauHoi.DapAnDung,
                        ct.CauHoi.MediaUrl,
                        LoaiBangLai = new
                        {
                            ct.CauHoi.LoaiBangLai.Id,
                            ct.CauHoi.LoaiBangLai.TenLoai,
                            ct.CauHoi.LoaiBangLai.ThoiGianThi,
                            ct.CauHoi.LoaiBangLai.DiemToiThieu
                        }
                    }
                }).ToList()
            };

            return Ok(result);
        }


        [HttpGet("danh-sach-bai-thi")]
        public async Task<IActionResult> GetDanhSachBaiThi()
        {
            var list = await _baiThiService.GetDanhSachBaiThi();
            return Ok(list);
        }

        [HttpGet("de-thi-ngau-nhien/{loaiBangLaiId}")]
        public async Task<IActionResult> GetDeThiNgauNhien(Guid loaiBangLaiId)
        {
            var deThi = await _baiThiService.GetDeThiNgauNhien(loaiBangLaiId);
            if (deThi == null)
                return NotFound(new { success = false, message = "Không tìm thấy đề thi phù hợp!" });

            return Ok(deThi);
        }

        [HttpGet("danh-sach-de-thi")]
        public async Task<IActionResult> GetDanhSachDeThi([FromQuery] string loaiXe = null)
        {
            var list = await _baiThiService.GetDanhSachDeThi(loaiXe);
            return Ok(list);
        }

        [HttpGet("on-tap/{loaiBangLaiId}")]
        public async Task<IActionResult> GetCauHoiOnTap(Guid loaiBangLaiId)
        {
            var list = await _baiThiService.GetCauHoiOnTap(loaiBangLaiId);
            return Ok(list);
        }
        [HttpGet("cau-hoi")]
        public async Task<IActionResult> GetCauHoiTheoChuDe(Guid loaiBangLaiId, Guid chuDeId)
        {
            var list = await _chuDeService.GetCauHoiTheoChuDe(loaiBangLaiId, chuDeId);
            return Ok(list);
        }

        [HttpGet("de-thi/{loaiBangLaiId}")]
        public async Task<IActionResult> GetDeThiByLoaiBangLai(Guid loaiBangLaiId)
        {
            var loaiBangLai = await _loaiBangLaiService.GetByIdAsync(loaiBangLaiId);
            if (loaiBangLai == null)
                return NotFound("Không tìm thấy loại bằng lái.");

            var danhSachDeThi = await _baiThiService.GetDeThiByLoaiBangLai(loaiBangLaiId);

            var result = new
            {
                loai = new
                {
                    loaiBangLai.Id,
                    loaiBangLai.TenLoai
                },
                danhSachDeThi = danhSachDeThi.Select(bt => new
                {
                    bt.Id,
                    bt.TenBaiThi,
                    SoCau = bt.ChiTietBaiThis?.Count ?? 0,                  
                })
            };

            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetChiTietBaiThi(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new { success = false, message = "ID không hợp lệ!" });
            }

            var baiThi = await _baiThiService.GetChiTietBaiThi(id);

            if (baiThi == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy bài thi." });
            }

            return Ok(new { success = true, data = baiThi });
        }
    }
}
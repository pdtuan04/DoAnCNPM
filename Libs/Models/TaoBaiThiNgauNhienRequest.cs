using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libs.Models
{
    public class TaoBaiThiNgauNhienRequest
    {
        public required string TenBaiThi { get; set; }
        public Guid LoaiBangLaiId { get; set; }
        public required IEnumerable<SoLuongCauHoiTheoChuDe> SoLuongCauHoiTheoChuDe { get; set; }
    }

    public class SoLuongCauHoiTheoChuDe
    {
        public Guid ChuDeId { get; set; }
        public int SoLuong { get; set; }
    }
}

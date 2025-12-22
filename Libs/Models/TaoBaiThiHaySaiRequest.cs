using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libs.Models
{
    public class TaoBaiThiHaySaiRequest
    {
        public required string TenBaiThi { get; set; }
        public required int SoLuong { get; set; }
    }
}

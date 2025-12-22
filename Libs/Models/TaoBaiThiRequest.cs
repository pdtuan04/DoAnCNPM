using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libs.Models
{
    public class TaoBaiThiRequest
    {
        public required String TenBaiThi { get; set; }
        public required List<Guid> CauHoiIds { get; set; }
    }
}

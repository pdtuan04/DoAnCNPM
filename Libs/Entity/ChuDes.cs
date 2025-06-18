using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libs.Entity
{
    public class ChuDes
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string TenChuDe { get; set; }
        public string MoTa { get; set; }
        public string MediaUrl { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libs.Entity
{
    public class LoaiBangLais
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string TenLoaiBangLai { get; set; }
        public string MoTa { get; set; }    
        public string LoaiXe { get; set; }
        public int ThoiGianThi { get; set; }
        public int DiemToiThieu { get; set; }
        public ICollection<Questions> Questions { get; set; } = new List<Questions>();

        // Many-to-many relationship with ChuDes if needed
    }
}

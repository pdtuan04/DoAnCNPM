using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libs.Entity
{
    public class LoaiBangLai
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string TenLoai { get; set; }

        public string MoTa { get; set; }
        //them 3 cai nay 
        public string LoaiXe { get; set; }
        public int ThoiGianThi { get; set; }
        public int DiemToiThieu { get; set; }
        //
        public bool isDeleted { get; set; } = false;
        [ValidateNever]
        public ICollection<CauHoi> CauHois { get; set; }
        [ValidateNever]
        public ICollection<BaiSaHinh> BaiSaHinhs { get; set; }
    }
}

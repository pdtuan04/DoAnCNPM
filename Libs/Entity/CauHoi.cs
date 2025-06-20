﻿using System.ComponentModel.DataAnnotations;

namespace Libs.Entity
{
    public class CauHoi
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string NoiDung { get; set; }

        [Required]
        public string LuaChonA { get; set; }

        [Required]
        public string LuaChonB { get; set; }

        public string? LuaChonC { get; set; }

        public string? LuaChonD { get; set; }

        [Required]
        public char DapAnDung { get; set; }

        public string? GiaiThich { get; set; }

        public bool DiemLiet { get; set; } = false;

        public string? MediaUrl { get; set; }

        public string? LoaiMedia { get; set; }

        public string? MeoGhiNho { get; set; }

        public bool isDeleted { get; set; } = false;
        ////them vao  
        //public string LoaiCauHoi { get; set; }
        public Guid ChuDeId { get; set; }
        public ChuDe? ChuDe { get; set; }

        public Guid LoaiBangLaiId { get; set; }
        public LoaiBangLai? LoaiBangLai { get; set; }
        public ICollection<ChiTietBaiThi>? ChiTietBaiThis { get; set; }
        public ICollection<ChiTietLichSuThi>? ChiTietLichSuThis { get; set; }
    }
}

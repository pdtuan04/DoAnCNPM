using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Libs.Entity
{
    public class Questions
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Content { get; set; }
        public string AnswerA { get; set; }
        public string AnswerB { get; set; }
        public string AnswerC { get; set; }
        public string AnswerD { get; set; }
        public string CorrectAnswer { get; set; }
        public string MediaUrl { get; set; }
        public bool IsDelete { get; set; } = false;
        public Guid ChuDeId { get; set; }
        public Guid LoaiBangLaiId { get; set; }
        public ChuDes ChuDe { get; set; }
        public LoaiBangLais LoaiBangLai { get; set; }
    }
}

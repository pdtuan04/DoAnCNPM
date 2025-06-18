using System;
using System.ComponentModel.DataAnnotations;

namespace ET.ViewModels
{
    public class QuestionViewModel
    {
        public Guid Id { get; set; }

        public string Content { get; set; }

        public string AnswerA { get; set; }

        public string AnswerB { get; set; }

        public string AnswerC { get; set; }

        public string AnswerD { get; set; }

        public string CorrectAnswer { get; set; }

        public string MediaUrl { get; set; }

        public Guid ChuDeId { get; set; }

        public Guid LoaiBangLaiId { get; set; }
    }
}
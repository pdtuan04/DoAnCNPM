using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ET.ViewModels;
using Libs.Entity;
using Libs.Repositories;

namespace Libs.Service
{
    public class QuestionService
    {
        private ApplicationDbContext _dbContext;
        private IQuestionRepository _questionRepository;

        public QuestionService(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
            this._questionRepository = new QuestionRepository(dbContext);
        }

        public List<Questions> GetAllQuestions()
        {
            return _questionRepository.GetMany(q => !q.IsDelete).ToList();
        }

        public Questions GetQuestionById(Guid id)
        {
            return _questionRepository.GetById(id);
        }

        public List<Questions> GetQuestionsByLoaiBangLai(Guid loaiBangLaiId, int count)
        {
            return _questionRepository.GetQuestionsByLoaiBangLai(loaiBangLaiId, count).ToList();
        }

        public List<Questions> GetQuestionByChuDe(Guid chuDeId, int count)
        {
            return _questionRepository.GetQuestionByChuDe(chuDeId, count).ToList();
        }
        public Questions MapFromViewModel(QuestionViewModel viewModel)
        {
            return new Questions
            {
                Content = viewModel.Content,
                AnswerA = viewModel.AnswerA,
                AnswerB = viewModel.AnswerB,
                AnswerC = viewModel.AnswerC,
                AnswerD = viewModel.AnswerD,
                CorrectAnswer = viewModel.CorrectAnswer,
                MediaUrl = viewModel.MediaUrl,
                IsDelete = false, // Mặc định là false khi tạo mới
                ChuDeId = viewModel.ChuDeId,
                LoaiBangLaiId = viewModel.LoaiBangLaiId
            };
        }
        public void CreateQuestion(QuestionViewModel viewModel)
        {
            var question = MapFromViewModel(viewModel);
            _questionRepository.Add(question);
            _questionRepository.Save();
        }

        public void UpdateQuestion(Questions question)
        {
            _questionRepository.Update(question);
            _questionRepository.Save();
        }

        public void DeleteQuestion(Guid id)
        {
            var question = _questionRepository.GetById(id);
            if (question != null)
            {
                question.IsDelete = true;
                _questionRepository.Update(question);
                _questionRepository.Save();
            }
        }

        public bool IsQuestionExists(Guid id)
        {
            return _questionRepository.Any(q => q.Id == id);
        }
    }
}
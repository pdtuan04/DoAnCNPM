using ET.ViewModels;
using Libs.Entity;
using Libs.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ET.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private QuestionService _questionService;
        public QuestionController(QuestionService questionService)
        {
            this._questionService = questionService;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("get-question-list")]
        public IActionResult GetQuestionList()
        {
            List<Questions> questionList = _questionService.GetAllQuestions();
            return Ok(new { status = true, message = "Get Question Successfully", data = questionList });
        }

        [HttpGet("get-question-by-id/{id}")]
        public IActionResult GetQuestionById(Guid id)
        {
            Questions question = _questionService.GetQuestionById(id);
            if (question == null)
            {
                return NotFound(new { status = false, message = "Question not found" });
            }
            return Ok(new { status = true, message = "Get Question Successfully", data = question });
        }

        [HttpGet("get-questions-by-loai-bang-lai/{loaiBangLaiId}/{count}")]
        public IActionResult GetQuestionsByLoaiBangLai(Guid loaiBangLaiId, int count)
        {
            List<Questions> questions = _questionService.GetQuestionsByLoaiBangLai(loaiBangLaiId, count);
            return Ok(new { status = true, message = "Get Questions Successfully", data = questions });
        }

        [HttpGet("get-questions-by-chu-de/{chuDeId}/{count}")]
        public IActionResult GetQuestionsByChuDe(Guid chuDeId, int count)
        {
            List<Questions> questions = _questionService.GetQuestionByChuDe(chuDeId, count);
            return Ok(new { status = true, message = "Get Questions Successfully", data = questions });
        }

        [HttpPost("create-question")]
        public IActionResult CreateQuestion(QuestionViewModel questionVM)
        {
            _questionService.CreateQuestion(questionVM);
            return Ok(new { status = true, message = "Create Question Successfully" });
        }

        [HttpPut("update-question")]
        public IActionResult UpdateQuestion(Questions question)
        {
            if (!_questionService.IsQuestionExists(question.Id))
            {
                return NotFound(new { status = false, message = "Question not found" });
            }
            _questionService.UpdateQuestion(question);
            return Ok(new { status = true, message = "Update Question Successfully" });
        }

        [HttpDelete("delete-question/{id}")]
        public IActionResult DeleteQuestion(Guid id)
        {
            if (!_questionService.IsQuestionExists(id))
            {
                return NotFound(new { status = false, message = "Question not found" });
            }
            _questionService.DeleteQuestion(id);
            return Ok(new { status = true, message = "Delete Question Successfully" });
        }
    }
}
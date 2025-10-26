using Libs.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ET.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatBoxController : ControllerBase
    {
        public readonly ChatBoxService _chatBoxService;
        public ChatBoxController(ChatBoxService aIChatBox)
        {
            _chatBoxService = aIChatBox;
        }
        [HttpGet("get-response")]
        public async Task<IActionResult> GetAIResponse([FromQuery] string prompt)
        {
            var response = await _chatBoxService.GetAIResponseAsync(prompt);
            return Ok(response);
        }
    }
}

using DoAn4.DTOs.UserDTO;
using DoAn4.Services.ConversationService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DoAn4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConversationController : ControllerBase
    {
        private readonly IConversationService _conversationService;

        public ConversationController(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        [HttpGet("get-conversation"), Authorize]
        public async Task<IActionResult> GetAllConversation()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _conversationService.GetAllConversations(token);
            if (result == null)
            {
                return Ok("Không có cuộc trò chuyện nào");
            }
            return Ok(result);
        }

        [HttpDelete("delete-conversation"), Authorize]
        public async Task<IActionResult> DeleteConversation( [FromBody] Guid conversationId)
        {
            
            var result = await _conversationService.DeleteConversation(conversationId);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }
    }
}

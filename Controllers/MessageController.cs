
using DoAn4.Services.MessageService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        //[HttpPost("send-message/{reciverId}"), Authorize]
        //public async Task<ActionResult> SendMessage([FromRoute]Guid reciverId, [FromForm] string content)
        //{
        //    var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        //    var result = await _messageService.SendMessage(token, reciverId, content);

        //    if (result == null)
        //    {
        //        return BadRequest("Gửi tin nhắn thất bại");
        //    }
        //    return Ok(result);
        //}

        [HttpGet("get-message/{reciverId}"), Authorize]
        public async Task<ActionResult> GetMessage([FromRoute] Guid reciverId)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _messageService.GetMessages(token, reciverId);

            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        [HttpDelete("delete-message/{messageId}"), Authorize]
        public async Task<ActionResult> DeleteMessage([FromRoute] Guid messageId)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _messageService.DeleteMessage(token, messageId);

            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }
    } 
}

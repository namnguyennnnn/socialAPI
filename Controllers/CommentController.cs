using DoAn4.DTOs.UserDTO;
using DoAn4.Services.CommentService;
using DoAn4.Services.UserOTPService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DoAn4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }
        
        [HttpGet("list-comment/{postId}")]
        public async Task<IActionResult> GetListComment([FromRoute] Guid postId)
        {
            var result = await _commentService.GetAllCommentAsync(postId);
            if (result == null)
            {
                return Ok("Chưa có bình luận nào");
            }
            return Ok(result);
        }

        [HttpPost("{postId}"), Authorize]
        public async Task<IActionResult> CreateComment([FromRoute] Guid postId, [FromForm] string content)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _commentService.CreateCommentAsync(token, postId, content);
            if (result == null)
            {
                return BadRequest("Thêm bình luận thất bại");
            }
            return Ok(result);
        }

        [HttpPut("update-comment/{commentId}"), Authorize]
        public async Task<IActionResult> UpdateComment([FromRoute] Guid commentId, [FromForm] string content)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _commentService.UpdateCommentAsync(token, commentId, content);
            if (result == null)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("delete-comment/{postId}/{commentId}"),Authorize]
        public async Task<IActionResult> DeleteComment([FromRoute] Guid postId, [FromRoute] Guid commentId)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _commentService.DeleteCommentAsync(token, postId, commentId);
            if (result == null)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}

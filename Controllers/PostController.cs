using DoAn4.DTOs.PostDTO;
using DoAn4.Interfaces;
using DoAn4.Services.PostService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DoAn4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }
       
        [HttpPost("add-post"), Authorize]
        public async Task<IActionResult> CreatePost([FromForm] PostDto postDto)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var post = await _postService.CreatePostAsync(token, postDto);
                return Ok(post);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating post: {ex.Message}");
            }
        }

        [HttpPut("update-post/{postId}"), Authorize]
        public async Task<IActionResult> UpdatePost(Guid postId, [FromForm] UpdatePostDto updatePostDto)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _postService.UpdatePostAsync(token, postId, updatePostDto);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        [HttpPut("delete-post/{postId}"), Authorize]
        public async Task<IActionResult> DeletePost(Guid postId)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var result = await _postService.DeletePostAsync(token, postId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error delete post: {ex.Message}");
            }
        }

        [HttpGet("get-post") ,Authorize]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetFriendPosts(int skip=0, int take = 5 )
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _postService.GetFriendPostsAsync(token, skip, take);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }

        [HttpGet("getPostOfUser/{userId}"), Authorize]
        public async Task<IActionResult> GetSelfPost([FromRoute] Guid userId)
        {
            
            var result = await _postService.GetSelfPostsAsync(userId);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }

    }
}

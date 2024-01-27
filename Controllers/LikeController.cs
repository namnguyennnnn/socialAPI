using DoAn4.DTOs;
using DoAn4.Services.LikeService;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;


namespace DoAn4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikeController : ControllerBase
    {

        private readonly ILikeService _likeService;

        public LikeController(ILikeService likeService)
        {
            _likeService = likeService;
        }

        [HttpPut("{postId}"),Authorize]
        public async Task<IActionResult> UpdatePostLikeStatus([FromRoute] Guid postId, [FromForm] LikeDto request)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var result = await _likeService.UpdatePostLikeStatusAsync(token, postId, request);

            if (result== null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpGet("{postId}/list-userlike")]

        public async Task<IActionResult> GetAll([FromRoute]Guid postId)
        {
           
            var result = await _likeService.GetInfoUsersLikedAsync( postId);

            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

    }
}

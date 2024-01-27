
using DoAn4.Services.FriendshipService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendController : ControllerBase
    {
        private readonly IFriendshipService _friendshipService;
       

        public FriendController(IFriendshipService friendshipService)
        {
            _friendshipService = friendshipService;
           
        }

        [HttpPost("add-friendships/{friendUserId}"),Authorize]
        public async Task<IActionResult> CreateFriendshipRequest([FromRoute] Guid friendUserId)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var result = await _friendshipService.SendFriendRequest(token, friendUserId);
                if (result == null)
                {
                    return BadRequest("Hai người đã là bạn hoặc yêu cầu kết bạn đã tồn tại");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });

            }
        }

        [HttpPut("accept-friendships/{friendShipId}"), Authorize]
        public async Task<IActionResult> AcceptFriendshipRequest([FromRoute] Guid friendShipId)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var result = await _friendshipService.AcceptFriendRequest(token, friendShipId);
                if (result == null)
                {
                    return BadRequest();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });

            }
        }

        [HttpDelete("reject-friendships/{friendShipId}"),Authorize]
        public async Task<IActionResult> RejectFriendshipRequest([FromRoute] Guid friendShipId)
        {
            try
            {               
                var result = await _friendshipService.RejectFriendRequest(friendShipId);
                if (result == null)
                {
                    return BadRequest();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });

            }
        }

        [HttpGet("listPending-friendships"),Authorize]
        public async Task<IActionResult> GetListFriendshipPending()
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var result = await _friendshipService.GetListFriendshipPendingAsync(token);
                if (result == null)
                {
                    return BadRequest();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });

            }
        }

        [HttpGet("list-friendships/{userId}"),Authorize]
        public async Task<IActionResult> GetListFriendship([FromRoute] Guid userId)
        {
            try
            {               
                var result = await _friendshipService.GetListFriendshipAsync(userId);
                return Ok(result);           
                
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });

            }
        }

        [HttpDelete("remove-friendships/{friendsUserId}"), Authorize]
        public async Task<IActionResult> DeleteFriendship(Guid friendsUserId)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var result = await _friendshipService.DeleteFriendship(token, friendsUserId);
                if (result == null)
                {
                    return BadRequest();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });

            }
        }

        [HttpDelete("revoke-friendships/{friendUserId}"), Authorize]
        public async Task<IActionResult> RevokeFriendshipRequest([FromRoute] Guid friendUserId)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var result = await _friendshipService.DeleteFriendship(token, friendUserId);
                if (result == null)
                {
                    return BadRequest();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });

            }
        }



    }
}

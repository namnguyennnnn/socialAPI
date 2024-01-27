
using DoAn4.DTOs.UserDTO;
using DoAn4.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("getProfile-user/{userId}"), Authorize]
        public async Task<IActionResult> GetProfileUser([FromRoute]Guid userId)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var user = await _userService.GetProfileUserAsync(token, userId);
                if (user == null)
                {
                    return BadRequest();
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating post: {ex.Message}");
            }
        }

        [HttpGet("getProfileFromToken-user"), Authorize]
        public async Task<IActionResult> GetProfileUserByToken( )
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var user = await _userService.GetProfileUserByTokenAsync(token);
                if (user == null)
                {
                    return BadRequest();
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating post: {ex.Message}");
            }
        }
        [HttpPut("updateProfile-user"), Authorize]
        public async Task<IActionResult> updateProfile([FromForm] UpdateProFileUserDto? updateProFileUserDto = null)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var user = await _userService.UpdateUserProfileAsync(token, updateProFileUserDto);
                if (user == null)
                {
                    return BadRequest();
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating post: {ex.Message}");
            }
        }
       
    }
}

using DoAn4.Services.NotificationService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotifyController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotifyController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("get-notify"), Authorize]
        public async Task<IActionResult> test()
        {

            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _notificationService.GetAllNotifies(token);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);

        }
    }
}

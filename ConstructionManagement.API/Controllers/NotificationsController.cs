using ConstructionManagement.BLL.DTOs;
using ConstructionManagement.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ConstructionManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationService _service;

        public NotificationsController(NotificationService service)
        {
            _service = service;
        }

        private bool TryGetUserId(out int userId)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);

            return int.TryParse(claim?.Value, out userId);
        }

        // 1. Create Notification
        [HttpPost]
        [Authorize(Roles = "Admin,Builder,Supervisor")]
        public async Task<IActionResult> CreateNotification(
            [FromBody] CreateNotificationRequest request)
        {
            try
            {
                var id = await _service.CreateNotificationAsync(request);

                return Ok(new
                {
                    message = "Notification created successfully.",
                    notificationId = id
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 2. Get Logged-in User Notifications
        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            if (!TryGetUserId(out int userId))
                return Unauthorized();

            var result = await _service.GetNotificationsAsync(userId);

            return Ok(result);
        }

        // 3. Mark Single Notification as Read
        [HttpPut("{id:long}/read")]
        public async Task<IActionResult> MarkAsRead(long id)
        {
            if (!TryGetUserId(out int userId))
                return Unauthorized();

            var result = await _service.MarkAsReadAsync(id, userId);

            if (!result)
                return NotFound(new
                {
                    message = "Notification not found."
                });

            return Ok(new
            {
                message = "Notification marked as read."
            });
        }

        // 4. Mark All Notifications as Read
        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            if (!TryGetUserId(out int userId))
                return Unauthorized();

            var count = await _service.MarkAllAsReadAsync(userId);

            return Ok(new
            {
                message = "All notifications marked as read.",
                updatedCount = count
            });
        }

        // 5. Get Unread Count
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            if (!TryGetUserId(out int userId))
                return Unauthorized();

            var count = await _service.GetUnreadCountAsync(userId);

            return Ok(new
            {
                unreadCount = count
            });
        }
    }
}
using FoodDeliveryApp03.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PayPalCheckoutSdk.Orders;
using System.Security.Claims;

namespace FoodDeliveryApp03.Controllers
{
    [ApiController]
    [Route("Notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly DBContext _context;

        public NotificationsController(DBContext context)
        {
            _context = context;
        }

        [HttpGet("GetNotifications")]
        public async Task<IActionResult> GetNotifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var notifications = await _context.Notifications
                .Where(notification => notification.UserId == userId) 
                .ToListAsync();
            return Ok(notifications);
        }


        [HttpGet("DeleteNotification/{notificationId}")]
        public async Task<IActionResult> DeleteNotification(int notificationId)
        {
            var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId);

            if (notification == null)
            {
                return NotFound($"Notification with ID {notificationId} not found.");
            }

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            return Ok($"Notification with ID {notificationId} deleted.");
        }
        [HttpPost("DeleteAllNotifications")]
        public async Task<IActionResult> DeleteAllNotifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var oldestNotification = await _context.Notifications
                .Where(notification => notification.UserId == userId)
                .OrderBy(notification => notification.CreatedAt)
                .FirstOrDefaultAsync();

            if (oldestNotification == null)
            {
                return Ok("No notifications to delete.");
            }

            _context.Notifications.Remove(oldestNotification);
            await _context.SaveChangesAsync();

            return RedirectToAction("Dashboard", "Carts");

        }

    }

}

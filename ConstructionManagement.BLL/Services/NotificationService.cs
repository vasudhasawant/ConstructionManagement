using ConstructionManagement.BLL.DTOs;
using ConstructionManagement.DAL.Context;
using ConstructionManagement.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConstructionManagement.BLL.Services
{
    public class NotificationService
    {
        private readonly ConstructionDbContext _context;

        public NotificationService(ConstructionDbContext context)
        {
            _context = context;
        }

        // 1. Create Notification
        public async Task<long> CreateNotificationAsync(
            CreateNotificationRequest request)
        {
            if (request.UserId <= 0)
                throw new ArgumentException("Invalid UserId.");

            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ArgumentException("Title is required.");

            if (string.IsNullOrWhiteSpace(request.Message))
                throw new ArgumentException("Message is required.");

            if (string.IsNullOrWhiteSpace(request.NotificationType))
                throw new ArgumentException("NotificationType is required.");

            if (request.Title.Length > 200 ||
                request.Message.Length > 1000 ||
                request.NotificationType.Length > 50 ||
                (request.ReferenceType?.Length ?? 0) > 50)
                throw new ArgumentException("One or more fields exceed the allowed length.");

            var userExists = await _context.Users
                .AnyAsync(x => x.UserId == request.UserId && x.IsActive);

            if (!userExists)
                throw new ArgumentException("User not found or inactive.");

            var notification = new Notification
            {
                UserId = request.UserId,
                Title = request.Title.Trim(),
                Message = request.Message.Trim(),
                NotificationType = request.NotificationType.Trim(),
                ReferenceId = request.ReferenceId,
                ReferenceType = request.ReferenceType?.Trim(),
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();

            return notification.NotificationId;
        }

        // 2. Get Notifications for Logged-in User
        public async Task<List<NotificationResponse>> GetNotificationsAsync(
            int userId)
        {
            return await _context.Notifications
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new NotificationResponse
                {
                    NotificationId = x.NotificationId,
                    UserId = x.UserId,
                    Title = x.Title,
                    Message = x.Message,
                    NotificationType = x.NotificationType,
                    ReferenceId = x.ReferenceId,
                    ReferenceType = x.ReferenceType,
                    IsRead = x.IsRead,
                    CreatedAt = x.CreatedAt,
                    ReadAt = x.ReadAt
                })
                .ToListAsync();
        }

        // 3. Mark Single Notification as Read
        public async Task<bool> MarkAsReadAsync(
            long notificationId, int userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(x =>
                    x.NotificationId == notificationId &&
                    x.UserId == userId);

            if (notification == null)
                return false;

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }

            return true;
        }

        // 4. Mark All Notifications as Read
        public async Task<int> MarkAllAsReadAsync(int userId)
        {
            var notifications = await _context.Notifications
                .Where(x => x.UserId == userId && !x.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return notifications.Count;
        }

        // 5. Get Unread Notification Count
        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _context.Notifications
                .CountAsync(x => x.UserId == userId && !x.IsRead);
        }
    }
}
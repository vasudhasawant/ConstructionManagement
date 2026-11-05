namespace ConstructionManagement.BLL.DTOs
{
    public class NotificationResponse
    {
        public long NotificationId { get; set; }

        public int UserId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string NotificationType { get; set; } = string.Empty;

        public long? ReferenceId { get; set; }

        public string? ReferenceType { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ReadAt { get; set; }
    }
}
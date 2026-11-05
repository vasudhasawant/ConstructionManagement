namespace ConstructionManagement.BLL.DTOs
{
    public class CreateNotificationRequest
    {
        public int UserId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string NotificationType { get; set; } = string.Empty;

        public long? ReferenceId { get; set; }

        public string? ReferenceType { get; set; }
    }
}
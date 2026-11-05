using System;
using System.Collections.Generic;

namespace ConstructionManagement.DAL.Entities;

public partial class Notification
{
    public long NotificationId { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string NotificationType { get; set; } = null!;

    public long? ReferenceId { get; set; }

    public string? ReferenceType { get; set; }

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ReadAt { get; set; }

    public virtual User User { get; set; } = null!;
}

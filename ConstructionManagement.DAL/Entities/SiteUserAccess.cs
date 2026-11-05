using System;
using System.Collections.Generic;

namespace ConstructionManagement.DAL.Entities;

public partial class SiteUserAccess
{
    public int SiteId { get; set; }

    public int UserId { get; set; }

    public int AssignedBy { get; set; }

    public DateTime AssignedAt { get; set; }

    public virtual User AssignedByNavigation { get; set; } = null!;

    public virtual ConstructionSite Site { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace ConstructionManagement.DAL.Entities;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? Email { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<ConstructionSite> ConstructionSites { get; set; } = new List<ConstructionSite>();

    public virtual Contractor? Contractor { get; set; }

    public virtual ICollection<MaterialDelivery> MaterialDeliveries { get; set; } = new List<MaterialDelivery>();

    public virtual ICollection<MaterialDemand> MaterialDemands { get; set; } = new List<MaterialDemand>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<SiteUserAccess> SiteUserAccessAssignedByNavigations { get; set; } = new List<SiteUserAccess>();

    public virtual ICollection<SiteUserAccess> SiteUserAccessUsers { get; set; } = new List<SiteUserAccess>();

    public virtual ICollection<WorkApproval> WorkApprovals { get; set; } = new List<WorkApproval>();
}

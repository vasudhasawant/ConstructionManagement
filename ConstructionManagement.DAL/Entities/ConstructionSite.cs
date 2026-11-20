using System;
using System.Collections.Generic;

namespace ConstructionManagement.DAL.Entities;

public partial class ConstructionSite
{
    public int SiteId { get; set; }

    public int BuilderId { get; set; }

    public string SiteName { get; set; } = null!;

    public string? Address { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User Builder { get; set; } = null!;

    public virtual ICollection<DailyWorkSubmission> DailyWorkSubmissions { get; set; } = new List<DailyWorkSubmission>();

    public virtual ICollection<MaterialDemand> MaterialDemands { get; set; } = new List<MaterialDemand>();

}

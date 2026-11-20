using System;
using System.Collections.Generic;

namespace ConstructionManagement.DAL.Entities;

public partial class MaterialDemand
{
    public long DemandId { get; set; }

    public int SiteId { get; set; }

    public int RequestedBy { get; set; }

    public DateTime DemandDate { get; set; }

    public DateOnly? RequiredDate { get; set; }

    //public string Status { get; set; } = null!;

    public int StatusId { get; set; }

    public string? Remarks { get; set; }

    public virtual ICollection<MaterialDemandItem> MaterialDemandItems { get; set; } = new List<MaterialDemandItem>();

    public virtual User RequestedByNavigation { get; set; } = null!;

    public virtual ConstructionSite Site { get; set; } = null!;

    public virtual MaterialDemandStatusMaster StatusNavigation { get; set; } = null!;
}

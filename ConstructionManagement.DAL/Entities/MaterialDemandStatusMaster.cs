
using System;
using System.Collections.Generic;

namespace ConstructionManagement.DAL.Entities;

public partial class MaterialDemandStatusMaster
{
    public int StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<MaterialDemand> MaterialDemands { get; set; }
        = new List<MaterialDemand>();
}

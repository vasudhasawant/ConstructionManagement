using System;
using System.Collections.Generic;

namespace ConstructionManagement.DAL.Entities;

public partial class MaterialDemandItem
{
    public long DemandItemId { get; set; }

    public long DemandId { get; set; }

    public string MaterialName { get; set; } = null!;

    public decimal RequestedQuantity { get; set; }

    public string Unit { get; set; } = null!;

    public decimal? ApprovedQuantity { get; set; }

    public decimal? UnitRate { get; set; }

    public virtual MaterialDemand Demand { get; set; } = null!;

    public virtual ICollection<MaterialDelivery> MaterialDeliveries { get; set; } = new List<MaterialDelivery>();
}

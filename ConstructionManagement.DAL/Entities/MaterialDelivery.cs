using System;
using System.Collections.Generic;

namespace ConstructionManagement.DAL.Entities;

public partial class MaterialDelivery
{
    public long DeliveryId { get; set; }

    public long DemandItemId { get; set; }

    public int ReceivedBy { get; set; }

    public decimal ReceivedQuantity { get; set; }

    public string? ChallanNumber { get; set; }

    public string ChallanPhotoPath { get; set; } = null!;

    public string? InvoicePhotoPath { get; set; }

    public string? InvoiceNumber { get; set; }

    public DateTime DeliveryDate { get; set; }

    public decimal? UnitRate { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? Remarks { get; set; }

    public virtual MaterialDemandItem DemandItem { get; set; } = null!;

    public virtual User ReceivedByNavigation { get; set; } = null!;
}

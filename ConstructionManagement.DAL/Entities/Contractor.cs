using System;
using System.Collections.Generic;

namespace ConstructionManagement.DAL.Entities;

public partial class Contractor
{
    public int ContractorId { get; set; }

    public int UserId { get; set; }

    public string? ContractorCompany { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<DailyWorkSubmission> DailyWorkSubmissions { get; set; } = new List<DailyWorkSubmission>();

    public virtual User User { get; set; } = null!;
}

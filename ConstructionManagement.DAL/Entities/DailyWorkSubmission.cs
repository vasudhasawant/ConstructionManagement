using System;
using System.Collections.Generic;

namespace ConstructionManagement.DAL.Entities;

public partial class DailyWorkSubmission
{
    public long SubmissionId { get; set; }

    public int SiteId { get; set; }

    public int ContractorId { get; set; }

    public DateOnly WorkDate { get; set; }

    public string AttendanceSelfiePath { get; set; } = null!;

    public string WorkPhotoPath { get; set; } = null!;

    public int DeclaredLabourCount { get; set; }

    public int? VerifiedLabourCount { get; set; }

    public string? WorkDescription { get; set; }

    public string? Latitude { get; set; }

    public string? Longitude { get; set; }

    public DateTime SubmittedAt { get; set; }

    public int StatusId { get; set; }

    public virtual WorkSubmissionStatusMaster StatusNavigation { get; set; } = null!;

    public virtual Contractor Contractor { get; set; } = null!;

    public virtual ConstructionSite Site { get; set; } = null!;

    public virtual ICollection<WorkApproval> WorkApprovals { get; set; } = new List<WorkApproval>();
}
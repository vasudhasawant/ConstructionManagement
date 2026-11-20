using System;
using System.Collections.Generic;

namespace ConstructionManagement.DAL.Entities;

public partial class WorkApproval
{
    public long ApprovalId { get; set; }

    public long SubmissionId { get; set; }

    public int ApprovalBy { get; set; }


    public int ApprovalLevel { get; set; }

    public string Decision { get; set; } = null!;

    public string? Remarks { get; set; }

    public DateTime DecisionAt { get; set; }


    public virtual DailyWorkSubmission Submission { get; set; } = null!;
    public virtual User ApprovalByNavigation { get; set; } = null!;
   
}

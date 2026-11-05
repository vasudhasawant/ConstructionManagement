using ConstructionManagement.DAL.Context;
using ConstructionManagement.DAL.Entities;
using ConstructionManagement.BLL.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ConstructionManagement.BLL.Services
{
    public class WorkApprovalService
    {
        private readonly ConstructionDbContext _context;

        public WorkApprovalService(ConstructionDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ApproveSubmissionAsync(
            long submissionId,
            int approverId,
            int approvalLevel,
            WorkApprovalRequest request)
        {
            // Validate decision
            if (request.Decision != "Approved" &&
                request.Decision != "Rejected")
            {
                throw new ArgumentException(
                    "Decision must be Approved or Rejected.");
            }

            // Validate approval level
            if (approvalLevel != 1 && approvalLevel != 2)
            {
                throw new ArgumentException(
                    "Approval level must be 1 or 2.");
            }

            // Find submission
            var submission = await _context.DailyWorkSubmissions
                .FirstOrDefaultAsync(x => x.SubmissionId == submissionId);

            if (submission == null)
                return false;

            // Check previous approval
            var existingApproval = await _context.WorkApprovals
                .AnyAsync(x =>
                    x.SubmissionId == submissionId &&
                    x.ApprovalLevel == approvalLevel);

            if (existingApproval)
            {
                throw new InvalidOperationException(
                    "This approval level has already been completed.");
            }

            // Level 2 can proceed only after Level 1 approval
            if (approvalLevel == 2)
            {
                var supervisorApproved = await _context.WorkApprovals
                    .AnyAsync(x =>
                        x.SubmissionId == submissionId &&
                        x.ApprovalLevel == 1 &&
                        x.Decision == "Approved");

                if (!supervisorApproved)
                {
                    throw new InvalidOperationException(
                        "Supervisor approval is required first.");
                }
            }

            // Create approval record
            var approval = new WorkApproval
            {
                SubmissionId = submissionId,
                ApproverId = approverId,
                ApprovalLevel = approvalLevel,
                Decision = request.Decision,
                Remarks = request.Remarks,
                DecisionAt = DateTime.UtcNow
            };

            _context.WorkApprovals.Add(approval);

            // Update submission status
            if (request.Decision == "Rejected")
            {
                submission.Status = "Rejected";
            }
            else if (approvalLevel == 1)
            {
                submission.Status = "PendingBuilder";
            }
            else
            {
                submission.Status = "Approved";
            }

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
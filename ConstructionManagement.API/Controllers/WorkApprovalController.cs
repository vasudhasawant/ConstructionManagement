using ConstructionManagement.BLL.DTOs;
using ConstructionManagement.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ConstructionManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Supervisor,Builder")]
    public class WorkApprovalController : ControllerBase
    {
        private readonly WorkApprovalService _workApprovalService;

        public WorkApprovalController(
            WorkApprovalService workApprovalService)
        {
            _workApprovalService = workApprovalService;
        }

        [HttpPost("{submissionId}/decision")]
        public async Task<IActionResult> ApproveSubmission(
            long submissionId,
            [FromBody] WorkApprovalRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null ||
                !int.TryParse(userIdClaim.Value, out int approverId))
            {
                return Unauthorized("Invalid user information.");
            }

            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            int approvalLevel = role switch
            {
                "Supervisor" => 1,
                "Builder" => 2,
                _ => 0
            };

            if (approvalLevel == 0)
                return Forbid();

            try
            {
                var result =
                    await _workApprovalService.ApproveSubmissionAsync(
                        submissionId,
                        approverId,
                        approvalLevel,
                        request);

                if (!result)
                    return NotFound("Work submission not found.");

                return Ok(new
                {
                    message = "Work approval decision submitted successfully."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
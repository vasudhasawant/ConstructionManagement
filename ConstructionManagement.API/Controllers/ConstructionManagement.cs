
using ConstructionManagement.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ConstructionManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly DashboardService _service;

        public ReportsController(DashboardService service)
        {
            _service = service;
        }

        private bool TryGetUser(out int userId, out string role)
        {
            var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var roleClaim = User.FindFirst(ClaimTypes.Role);

            role = roleClaim?.Value ?? "";

            return int.TryParse(userClaim?.Value, out userId)
                   && !string.IsNullOrWhiteSpace(role);
        }

        // GET: api/Reports/attendance
        [HttpGet("attendance")]
        public async Task<IActionResult> GetAttendanceReport(
            [FromQuery] int? siteId,
            [FromQuery] DateOnly? fromDate,
            [FromQuery] DateOnly? toDate)
        {
            if (!TryGetUser(out int userId, out string role))
                return Unauthorized();

            if (fromDate.HasValue && toDate.HasValue &&
                fromDate.Value > toDate.Value)
            {
                return BadRequest(new
                {
                    message = "From date cannot be greater than To date."
                });
            }

            var result = await _service.GetAttendanceReportAsync(
                userId, role, siteId, fromDate, toDate);

            return Ok(result);
        }

        // GET: api/Reports/materials
        [HttpGet("materials")]
        public async Task<IActionResult> GetMaterialReport(
            [FromQuery] int? siteId)
        {
            if (!TryGetUser(out int userId, out string role))
                return Unauthorized();

            var result = await _service.GetMaterialReportAsync(
                userId, role, siteId);

            return Ok(result);
        }
    }
}

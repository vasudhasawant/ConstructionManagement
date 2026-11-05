
using ConstructionManagement.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ConstructionManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _service;

        public DashboardController(DashboardService service)
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

        // GET: api/Dashboard/summary
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            if (!TryGetUser(out int userId, out string role))
                return Unauthorized();

            var result =
                await _service.GetDashboardSummaryAsync(userId, role);

            return Ok(result);
        }
    }
}

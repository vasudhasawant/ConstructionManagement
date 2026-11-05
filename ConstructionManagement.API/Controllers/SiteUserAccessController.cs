using System.Security.Claims;
using ConstructionManagement.BLL.DTOs;
using ConstructionManagement.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstructionManagement.API.Controllers
{
    [Authorize(Roles = "Admin,Builder")]
    [ApiController]
    [Route("api/[controller]")]
    public class SiteUserAccessController : ControllerBase
    {
        private readonly SiteUserAccessService _service;

        public SiteUserAccessController(SiteUserAccessService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> AssignUserToSite(
            [FromBody] AssignSiteUserRequest request)
        {
            try
            {
                var assignedByClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(assignedByClaim, out int assignedBy))
                    return Unauthorized("Invalid user token.");

                await _service.AssignUserToSiteAsync(request, assignedBy);

                return Ok(new
                {
                    message = "User assigned to site successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet("{siteId}")]
        public async Task<IActionResult> GetUsersBySite(int siteId)
        {
            try
            {
                var users = await _service.GetUsersBySiteAsync(siteId);

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpDelete("{siteId}/{userId}")]
        public async Task<IActionResult> RemoveUserFromSite(
    int siteId,
    int userId)
        {
            try
            {
                var result = await _service.RemoveUserFromSiteAsync(
                    siteId,
                    userId
                );

                if (!result)
                {
                    return NotFound(new
                    {
                        message = "User assignment not found."
                    });
                }

                return Ok(new
                {
                    message = "User removed from site successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
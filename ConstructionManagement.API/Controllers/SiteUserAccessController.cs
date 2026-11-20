
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstructionManagement.API.Controllers
{
    [Authorize(Roles = "Admin,Builder")]
    [ApiController]
    [Route("api/[controller]")]
    public class SiteUserAccessController : ControllerBase
    {
        [HttpPost]
        public IActionResult AssignUserToSite()
        {
            return StatusCode(501, new
            {
                message = "Site user assignment is currently unavailable because the SiteUserAccess table has been removed."
            });
        }

        [HttpGet("{siteId}")]
        public IActionResult GetUsersBySite(int siteId)
        {
            return StatusCode(501, new
            {
                message = "Site user listing is currently unavailable because the SiteUserAccess table has been removed."
            });
        }

        [HttpDelete("{siteId}/{userId}")]
        public IActionResult RemoveUserFromSite(
            int siteId,
            int userId)
        {
            return StatusCode(501, new
            {
                message = "Removing site user assignment is currently unavailable because the SiteUserAccess table has been removed."
            });
        }
    }
}

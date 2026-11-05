
using System.Security.Claims;
using ConstructionManagement.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstructionManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MenuController : ControllerBase
    {
        private readonly MenuService _menuService;

        public MenuController(MenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMenu()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrWhiteSpace(role))
            {
                return Unauthorized(new
                {
                    message = "User role not found in token."
                });
            }

            var allowedRoles = new[]
            {
                "Admin",
                "Builder",
                "Supervisor",
                "Contractor"
            };

            if (!allowedRoles.Contains(role))
            {
                return Forbid();
            }

            var result = await _menuService.GetMenuAsync(role);

            return Ok(result);
        }
    }
}

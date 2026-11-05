using ConstructionManagement.BLL.DTOs;
using System.Security.Claims;
using ConstructionManagement.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstructionManagement.API.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SitesController : ControllerBase
{
    private readonly SiteService _siteService;

    public SitesController(SiteService siteService)
    {
        _siteService = siteService;
    }

    [HttpGet]
    public async Task<IActionResult> GetSites()
    {
        var sites = await _siteService.GetSitesAsync();
        return Ok(sites);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSiteById(int id)
    {
        var site = await _siteService.GetSiteByIdAsync(id);

        if (site == null)
        {
            return NotFound("Site not found.");
        }

        return Ok(site);
    }

    [HttpPost]
    [Authorize(Roles = "Builder")]
    public async Task<IActionResult> CreateSite(
    [FromBody] CreateSiteRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.SiteName))
        {
            return BadRequest("Site name is required.");
        }

        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out int builderId))
        {
            return Unauthorized();
        }

        var site = await _siteService.CreateSiteAsync(request, builderId);

        return Ok(site);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Builder")]
    public async Task<IActionResult> UpdateSite(
    int id,
    [FromBody] UpdateSiteRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.SiteName))
        {
            return BadRequest("Site name is required.");
        }

        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out int builderId))
        {
            return Unauthorized();
        }

        var site = await _siteService.UpdateSiteAsync(
            id, request, builderId);

        if (site == null)
        {
            return NotFound("Site not found.");
        }

        return Ok(site);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Builder")]
    public async Task<IActionResult> DeactivateSite(int id)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out int builderId))
        {
            return Unauthorized();
        }

        var result = await _siteService.DeactivateSiteAsync(id, builderId);

        if (!result)
        {
            return NotFound("Site not found.");
        }

        return Ok("Site deactivated successfully.");
    }
}

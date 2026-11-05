using ConstructionManagement.BLL.DTOs;
using ConstructionManagement.DAL.Context;
using ConstructionManagement.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConstructionManagement.BLL.Services;

public class SiteService
{
    private readonly ConstructionDbContext _context;

    public SiteService(ConstructionDbContext context)
    {
        _context = context;
    }

    public async Task<List<SiteDto>> GetSitesAsync()
    {
        return await _context.ConstructionSites
            .AsNoTracking()
            .Select(site => new SiteDto
            {
                SiteId = site.SiteId,
                SiteName = site.SiteName,
                Address = site.Address,
                BuilderId = site.BuilderId
            })
            .ToListAsync();
    }


    public async Task<SiteDto?> GetSiteByIdAsync(int siteId)
    {
        return await _context.ConstructionSites
            .AsNoTracking()
            .Where(site => site.SiteId == siteId)
            .Select(site => new SiteDto
            {
                SiteId = site.SiteId,
                SiteName = site.SiteName,
                Address = site.Address,
                BuilderId = site.BuilderId
            })
            .FirstOrDefaultAsync();
    }

    public async Task<SiteDto> CreateSiteAsync(
       CreateSiteRequest request, int builderId)
    {
        var site = new ConstructionSite
        {
            SiteName = request.SiteName,
            Address = request.Address,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            BuilderId = builderId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.ConstructionSites.Add(site);
        await _context.SaveChangesAsync();

        return new SiteDto
        {
            SiteId = site.SiteId,
            SiteName = site.SiteName,
            Address = site.Address,
            BuilderId = site.BuilderId
        };
    }

    public async Task<SiteDto?> UpdateSiteAsync(
    int siteId,
    UpdateSiteRequest request,
    int builderId)
    {
        var site = await _context.ConstructionSites
            .FirstOrDefaultAsync(s =>
                s.SiteId == siteId &&
                s.BuilderId == builderId &&
                s.IsActive);

        if (site == null)
        {
            return null;
        }

        site.SiteName = request.SiteName;
        site.Address = request.Address;
        site.Latitude = request.Latitude;
        site.Longitude = request.Longitude;

        await _context.SaveChangesAsync();

        return new SiteDto
        {
            SiteId = site.SiteId,
            SiteName = site.SiteName,
            Address = site.Address,
            BuilderId = site.BuilderId
        };
    }

    public async Task<bool> DeactivateSiteAsync(int siteId, int builderId)
    {
        var site = await _context.ConstructionSites
            .FirstOrDefaultAsync(s =>
                s.SiteId == siteId &&
                s.BuilderId == builderId &&
                s.IsActive);

        if (site == null)
        {
            return false;
        }

        site.IsActive = false;

        await _context.SaveChangesAsync();

        return true;
    }
}

public class SiteDto
{
    public int SiteId { get; set; }
    public string SiteName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public int BuilderId { get; set; }
}



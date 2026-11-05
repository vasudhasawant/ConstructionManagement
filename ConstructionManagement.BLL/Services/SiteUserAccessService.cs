using ConstructionManagement.DAL.Context;
using ConstructionManagement.DAL.Entities;
using ConstructionManagement.BLL.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ConstructionManagement.BLL.Services
{
    public class SiteUserAccessService
    {
        private readonly ConstructionDbContext _context;

        public SiteUserAccessService(ConstructionDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AssignUserToSiteAsync(AssignSiteUserRequest request, int assignedBy)
        {
            // Check whether site exists
            var site = await _context.ConstructionSites
                .FirstOrDefaultAsync(x => x.SiteId == request.SiteId && x.IsActive);

            if (site == null)
                throw new Exception("Site not found.");

            // Check whether user exists
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.UserId == request.UserId && x.IsActive);

            if (user == null)
                throw new Exception("User not found.");

            // Check duplicate assignment
            var existingAccess = await _context.SiteUserAccesses
                .AnyAsync(x => x.SiteId == request.SiteId && x.UserId == request.UserId);

            if (existingAccess)
                throw new Exception("User is already assigned to this site.");

            var siteUserAccess = new SiteUserAccess
            {
                SiteId = request.SiteId,
                UserId = request.UserId,
                AssignedBy = assignedBy,
                AssignedAt = DateTime.UtcNow
            };

            _context.SiteUserAccesses.Add(siteUserAccess);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<object>> GetUsersBySiteAsync(int siteId)
        {
            var users = await _context.SiteUserAccesses
                .Where(x => x.SiteId == siteId && x.User.IsActive)
                .Select(x => new
                {
                    x.User.UserId,
                    x.User.FullName,
                    x.User.Phone,
                    x.User.Role
                })
                .ToListAsync();

            return users.Cast<object>().ToList();
        }

        public async Task<bool> RemoveUserFromSiteAsync(int siteId, int userId)
        {
            var access = await _context.SiteUserAccesses
                .FirstOrDefaultAsync(x =>
                    x.SiteId == siteId &&
                    x.UserId == userId);

            if (access == null)
                return false;

            _context.SiteUserAccesses.Remove(access);

            await _context.SaveChangesAsync();

            return true;
        }

    }
}
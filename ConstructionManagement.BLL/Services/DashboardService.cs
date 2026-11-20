
using ConstructionManagement.BLL.DTOs.Dashboard;
using ConstructionManagement.DAL.Context;
using Microsoft.EntityFrameworkCore;

namespace ConstructionManagement.BLL.Services
{
    public class DashboardService
    {
        private readonly ConstructionDbContext _context;

        public DashboardService(ConstructionDbContext context)
        {
            _context = context;
        }

        private async Task<List<int>> GetAccessibleSiteIdsAsync(
            int userId, string role)
        {
            if (role == "Admin")
            {
                return await _context.ConstructionSites
                    .Where(x => x.IsActive)
                    .Select(x => x.SiteId)
                    .ToListAsync();
            }

            if (role == "Builder")
            {
                return await _context.ConstructionSites
                    .Where(x => x.BuilderId == userId && x.IsActive)
                    .Select(x => x.SiteId)
                    .ToListAsync();
            }

            if (role == "Contractor")
            {
                return await _context.DailyWorkSubmissions
                    .Where(x => x.Contractor.UserId == userId)
                    .Select(x => x.SiteId)
                    .Distinct()
                    .ToListAsync();
            }

            return new List<int>();
        }

        public async Task<DashboardSummaryResponse> GetDashboardSummaryAsync(
            int userId, string role)
        {
            var siteIds = await GetAccessibleSiteIdsAsync(userId, role);

            var today = DateOnly.FromDateTime(DateTime.Today);

            var totalContractors = await _context.Contractors
                .Where(c => c.IsActive &&
                    _context.DailyWorkSubmissions.Any(s =>
                        s.ContractorId == c.ContractorId &&
                        siteIds.Contains(s.SiteId)))
                .Select(c => c.ContractorId)
                .Distinct()
                .CountAsync();

            var presentToday = await _context.DailyWorkSubmissions
                .CountAsync(x =>
                    siteIds.Contains(x.SiteId) &&
                    x.WorkDate == today);

            var pendingApprovals = await _context.DailyWorkSubmissions
                .CountAsync(x =>
                    siteIds.Contains(x.SiteId) &&
                    (x.StatusId == 1 || x.StatusId == 2));

            var totalDemands = await _context.MaterialDemands
                .CountAsync(x => siteIds.Contains(x.SiteId));

            var pendingDemands = await _context.MaterialDemands
                .CountAsync(x =>
                    siteIds.Contains(x.SiteId) &&
                    x.StatusId == 1);

            var totalAmount = await _context.MaterialDeliveries
                .Where(x =>
                    siteIds.Contains(x.DemandItem.Demand.SiteId))
                .SumAsync(x => (decimal?)x.TotalAmount) ?? 0;

            return new DashboardSummaryResponse
            {
                TotalContractors = totalContractors,
                PresentToday = presentToday,
                PendingApprovals = pendingApprovals,
                TotalMaterialDemands = totalDemands,
                PendingMaterialDemands = pendingDemands,
                TotalMaterialAmount = totalAmount
            };
        }

        public async Task<List<AttendanceReportResponse>>
            GetAttendanceReportAsync(
                int userId,
                string role,
                int? siteId = null,
                DateOnly? fromDate = null,
                DateOnly? toDate = null)
        {
            var siteIds = await GetAccessibleSiteIdsAsync(userId, role);

            if (siteId.HasValue)
                siteIds = siteIds.Where(x => x == siteId.Value).ToList();

            var query =
                from submission in _context.DailyWorkSubmissions.AsNoTracking()
                join contractor in _context.Contractors
                    on submission.ContractorId equals contractor.ContractorId
                join user in _context.Users
                    on contractor.UserId equals user.UserId
                join site in _context.ConstructionSites
                    on submission.SiteId equals site.SiteId
                where siteIds.Contains(submission.SiteId)
                select new AttendanceReportResponse
                {
                    SubmissionId = submission.SubmissionId,
                    SiteId = submission.SiteId,
                    SiteName = site.SiteName,
                    ContractorName = user.FullName,
                    WorkDate = submission.WorkDate,
                    DeclaredLabourCount = submission.DeclaredLabourCount,
                    VerifiedLabourCount = submission.VerifiedLabourCount,
                    Status = submission.StatusNavigation.StatusName,
                    WorkDescription = submission.WorkDescription
                };

            if (fromDate.HasValue)
                query = query.Where(x => x.WorkDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(x => x.WorkDate <= toDate.Value);

            return await query
                .OrderByDescending(x => x.WorkDate)
                .ToListAsync();
        }

        public async Task<List<MaterialReportResponse>>
            GetMaterialReportAsync(
                int userId,
                string role,
                int? siteId = null)
        {
            var siteIds = await GetAccessibleSiteIdsAsync(userId, role);

            if (siteId.HasValue)
                siteIds = siteIds.Where(x => x == siteId.Value).ToList();

            return await _context.MaterialDemandItems
                .AsNoTracking()
                .Where(x => siteIds.Contains(x.Demand.SiteId))
                .Select(x => new MaterialReportResponse
                {
                    DemandId = x.DemandId,
                    SiteId = x.Demand.SiteId,
                    SiteName = x.Demand.Site.SiteName,
                    MaterialName = x.MaterialName,
                    RequestedQuantity = x.RequestedQuantity,
                    ApprovedQuantity = x.ApprovedQuantity,
                    TotalReceivedQuantity =
                        x.MaterialDeliveries.Sum(d =>
                            (decimal?)d.ReceivedQuantity) ?? 0,
                    Unit = x.Unit,
                    DemandStatus = x.Demand.StatusNavigation.StatusName,
                    TotalAmount =
                        x.MaterialDeliveries.Sum(d =>
                            d.TotalAmount) ?? 0
                })
                .OrderByDescending(x => x.DemandId)
                .ToListAsync();
        }
    }
}

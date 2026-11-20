
using ConstructionManagement.BLL.DTOs;
using ConstructionManagement.DAL.Context;
using ConstructionManagement.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConstructionManagement.BLL.Services
{
    public class MaterialDemandService
    {
        private readonly ConstructionDbContext _context;

        public MaterialDemandService(ConstructionDbContext context)
        {
            _context = context;
        }

        // Create Material Demand
        public async Task<long> CreateDemandAsync(
            int requestedBy,
            CreateMaterialDemandRequest request)
        {
            if (request.Items == null || !request.Items.Any())
                throw new ArgumentException(
                    "At least one material item is required.");

            if (request.Items.Any(x =>
                string.IsNullOrWhiteSpace(x.MaterialName) ||
                string.IsNullOrWhiteSpace(x.Unit) ||
                x.RequestedQuantity <= 0))
            {
                throw new ArgumentException(
                    "Enter valid material names, units and quantities.");
            }

            var siteExists = await _context.ConstructionSites
                .AnyAsync(x => x.SiteId == request.SiteId && x.IsActive);

            if (!siteExists)
                throw new ArgumentException("Construction site not found.");

            var demand = new MaterialDemand
            {
                SiteId = request.SiteId,
                RequestedBy = requestedBy,
                DemandDate = DateTime.UtcNow,
                RequiredDate = request.RequiredDate,
                StatusId = 1,
                Remarks = request.Remarks
            };

            foreach (var item in request.Items)
            {
                demand.MaterialDemandItems.Add(new MaterialDemandItem
                {
                    MaterialName = item.MaterialName.Trim(),
                    RequestedQuantity = item.RequestedQuantity,
                    Unit = item.Unit.Trim()
                });
            }

            _context.MaterialDemands.Add(demand);

            await _context.SaveChangesAsync();

            return demand.DemandId;
        }

        // Get All Material Demands
        public async Task<List<object>> GetAllDemandsAsync()
        {
            var demands = await _context.MaterialDemands
                .Include(x => x.Site)
                .Include(x => x.RequestedByNavigation)
                .Include(x => x.StatusNavigation)
                .Include(x => x.MaterialDemandItems)
                .OrderByDescending(x => x.DemandDate)
                .ToListAsync();

            return demands.Select(d => (object)new
            {
                d.DemandId,
                d.SiteId,
                SiteName = d.Site.SiteName,
                d.RequestedBy,
                RequestedByName = d.RequestedByNavigation.FullName,
                d.DemandDate,
                d.RequiredDate,
                d.StatusId,
                Status = d.StatusNavigation.StatusName,
                d.Remarks,

                Items = d.MaterialDemandItems.Select(i => new
                {
                    i.DemandItemId,
                    i.MaterialName,
                    i.RequestedQuantity,
                    i.Unit,
                    i.ApprovedQuantity,
                    i.UnitRate
                }).ToList()
            }).ToList();
        }

        // Get Material Demand By ID
        public async Task<object?> GetDemandByIdAsync(long demandId)
        {
            var demand = await _context.MaterialDemands
                .Include(x => x.Site)
                .Include(x => x.RequestedByNavigation)
                .Include(x => x.StatusNavigation)
                .Include(x => x.MaterialDemandItems)
                .FirstOrDefaultAsync(x => x.DemandId == demandId);

            if (demand == null)
                return null;

            return new
            {
                demand.DemandId,
                demand.SiteId,
                SiteName = demand.Site.SiteName,
                demand.RequestedBy,
                RequestedByName = demand.RequestedByNavigation.FullName,
                demand.DemandDate,
                demand.RequiredDate,
                demand.StatusId,
                Status = demand.StatusNavigation.StatusName,
                demand.Remarks,

                Items = demand.MaterialDemandItems.Select(i => new
                {
                    i.DemandItemId,
                    i.MaterialName,
                    i.RequestedQuantity,
                    i.Unit,
                    i.ApprovedQuantity,
                    i.UnitRate
                }).ToList()
            };
        }

        // Update Material Demand Status
        public async Task<bool> UpdateDemandStatusAsync(
            long demandId,
            UpdateMaterialDemandStatusRequest request)
        {
            var allowedStatuses = new[]
            {
                "Approved",
                "Rejected"
            };

            if (!allowedStatuses.Contains(request.Status))
            {
                throw new ArgumentException(
                    "Status must be Approved or Rejected.");
            }

            var demand = await _context.MaterialDemands
                .Include(x => x.MaterialDemandItems)
                .FirstOrDefaultAsync(x => x.DemandId == demandId);

            if (demand == null)
                return false;

            if (demand.StatusId != 1)
            {
                throw new InvalidOperationException(
                    "Only pending demands can be approved or rejected.");
            }

            // Validate and save approved quantities
            if (request.Status == "Approved")
            {
                if (request.Items == null || !request.Items.Any())
                {
                    throw new ArgumentException(
                        "Approved quantities are required for all material items.");
                }

                if (request.Items.Count != demand.MaterialDemandItems.Count)
                {
                    throw new ArgumentException(
                        "Please provide an approved quantity for every material item.");
                }

                foreach (var itemRequest in request.Items)
                {
                    var demandItem = demand.MaterialDemandItems
                        .FirstOrDefault(x =>
                            x.DemandItemId == itemRequest.DemandItemId);

                    if (demandItem == null)
                    {
                        throw new ArgumentException(
                            "Invalid material demand item ID: " +
                            itemRequest.DemandItemId);
                    }

                    if (itemRequest.ApprovedQuantity <= 0)
                    {
                        throw new ArgumentException(
                            "Approved quantity must be greater than zero.");
                    }

                    if (itemRequest.ApprovedQuantity > demandItem.RequestedQuantity)
                    {
                        throw new ArgumentException(
                            "Approved quantity cannot exceed requested quantity for " +
                            demandItem.MaterialName + ".");
                    }

                    demandItem.ApprovedQuantity =
                        itemRequest.ApprovedQuantity;
                }
            }

            demand.StatusId =
                request.Status == "Approved" ? 2 : 3;

            if (!string.IsNullOrWhiteSpace(request.Remarks))
            {
                demand.Remarks = request.Remarks;
            }

            await _context.SaveChangesAsync();

            return true;
        }
    }
}

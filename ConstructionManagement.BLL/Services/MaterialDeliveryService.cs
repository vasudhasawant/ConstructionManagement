
using ConstructionManagement.BLL.DTOs;
using ConstructionManagement.DAL.Context;
using ConstructionManagement.DAL.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace ConstructionManagement.BLL.Services
{
    public class MaterialDeliveryService
    {
        private readonly ConstructionDbContext _context;
        private readonly IWebHostEnvironment _environment;

        private const long MaxFileSize = 5 * 1024 * 1024;

        private static readonly string[] AllowedExtensions =
        {
            ".jpg", ".jpeg", ".png", ".webp"
        };

        public MaterialDeliveryService(
            ConstructionDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // 1. CREATE MATERIAL DELIVERY
        public async Task<long> CreateDeliveryAsync(
            int receivedBy,
            CreateMaterialDeliveryRequest request)
        {
            if (request.ReceivedQuantity <= 0)
                throw new ArgumentException(
                    "Received quantity must be greater than zero.");

            if (request.UnitRate < 0)
                throw new ArgumentException(
                    "Unit rate cannot be negative.");

            ValidateImage(request.ChallanPhoto, "Challan photo");

            if (request.InvoicePhoto != null)
                ValidateImage(request.InvoicePhoto, "Invoice photo");

            var item = await _context.MaterialDemandItems
                .Include(x => x.Demand)
                .FirstOrDefaultAsync(
                    x => x.DemandItemId == request.DemandItemId);

            if (item == null)
                throw new ArgumentException(
                    "Material demand item not found.");

            if (item.Demand.StatusId != 2 &&
                item.Demand.StatusId != 4)
            {
                throw new InvalidOperationException(
                    "Only approved material demands can receive deliveries.");
            }

            decimal approvedQuantity =
                item.ApprovedQuantity ?? item.RequestedQuantity;

            var alreadyReceived = await _context.MaterialDeliveries
                .Where(x => x.DemandItemId == request.DemandItemId)
                .SumAsync(x => (decimal?)x.ReceivedQuantity) ?? 0;

            decimal remainingQuantity =
                approvedQuantity - alreadyReceived;

            if (request.ReceivedQuantity > remainingQuantity)
            {
                throw new ArgumentException(
                    "Received quantity exceeds remaining quantity: " +
                    remainingQuantity);
            }

            string? challanPath = null;
            string? invoicePath = null;

            try
            {
                challanPath = await SaveImageAsync(
                    request.ChallanPhoto);

                if (request.InvoicePhoto != null)
                {
                    invoicePath = await SaveImageAsync(
                        request.InvoicePhoto);
                }

                var delivery = new MaterialDelivery
                {
                    DemandItemId = request.DemandItemId,
                    ReceivedBy = receivedBy,
                    ReceivedQuantity = request.ReceivedQuantity,
                    ChallanNumber = request.ChallanNumber,
                    ChallanPhotoPath = challanPath,
                    InvoicePhotoPath = invoicePath,
                    InvoiceNumber = request.InvoiceNumber,
                    DeliveryDate = DateTime.UtcNow,
                    UnitRate = request.UnitRate,
                    TotalAmount = request.UnitRate.HasValue
                        ? request.ReceivedQuantity * request.UnitRate.Value
                        : null,
                    Remarks = request.Remarks
                };

                _context.MaterialDeliveries.Add(delivery);

                var demandItems = await _context.MaterialDemandItems
                    .Where(x => x.DemandId == item.DemandId)
                    .ToListAsync();

                bool allDelivered = true;

                foreach (var demandItem in demandItems)
                {
                    var received = await _context.MaterialDeliveries
                        .Where(x =>
                            x.DemandItemId == demandItem.DemandItemId)
                        .SumAsync(x => (decimal?)x.ReceivedQuantity) ?? 0;

                    if (demandItem.DemandItemId == request.DemandItemId)
                    {
                        received += request.ReceivedQuantity;
                    }

                    decimal approved =
                        demandItem.ApprovedQuantity ??
                        demandItem.RequestedQuantity;

                    if (received < approved)
                    {
                        allDelivered = false;
                        break;
                    }
                }

                item.Demand.StatusId = allDelivered ? 5 : 4;

                await _context.SaveChangesAsync();

                return delivery.DeliveryId;
            }
            catch
            {
                DeleteUploadedFile(challanPath);
                DeleteUploadedFile(invoicePath);

                throw;
            }
        }

        // 2. GET ALL MATERIAL DELIVERIES
        public async Task<List<object>> GetAllDeliveriesAsync()
        {
            var deliveries = await _context.MaterialDeliveries
                .Include(x => x.DemandItem)
                    .ThenInclude(x => x.Demand)
                        .ThenInclude(x => x.Site)
                .Include(x => x.ReceivedByNavigation)
                .OrderByDescending(x => x.DeliveryDate)
                .ToListAsync();

            return deliveries.Select(d => (object)new
            {
                d.DeliveryId,
                d.DemandItemId,
                d.DemandItem.DemandId,
                d.DemandItem.MaterialName,
                d.DemandItem.Unit,
                d.DemandItem.Demand.SiteId,
                SiteName = d.DemandItem.Demand.Site.SiteName,
                d.ReceivedBy,
                ReceivedByName = d.ReceivedByNavigation.FullName,
                d.ReceivedQuantity,
                d.ChallanNumber,
                d.ChallanPhotoPath,
                d.InvoicePhotoPath,
                d.InvoiceNumber,
                d.DeliveryDate,
                d.UnitRate,
                d.TotalAmount,
                d.Remarks
            }).ToList();
        }

        // 3. GET MATERIAL DELIVERY BY ID
        public async Task<object?> GetDeliveryByIdAsync(long deliveryId)
        {
            var d = await _context.MaterialDeliveries
                .Include(x => x.DemandItem)
                    .ThenInclude(x => x.Demand)
                        .ThenInclude(x => x.Site)
                .Include(x => x.ReceivedByNavigation)
                .FirstOrDefaultAsync(
                    x => x.DeliveryId == deliveryId);

            if (d == null)
                return null;

            return new
            {
                d.DeliveryId,
                d.DemandItemId,
                d.DemandItem.DemandId,
                d.DemandItem.MaterialName,
                d.DemandItem.Unit,
                d.DemandItem.Demand.SiteId,
                SiteName = d.DemandItem.Demand.Site.SiteName,
                d.ReceivedBy,
                ReceivedByName = d.ReceivedByNavigation.FullName,
                d.ReceivedQuantity,
                d.ChallanNumber,
                d.ChallanPhotoPath,
                d.InvoicePhotoPath,
                d.InvoiceNumber,
                d.DeliveryDate,
                d.UnitRate,
                d.TotalAmount,
                d.Remarks
            };
        }

        // IMAGE VALIDATION
        private static void ValidateImage(
            Microsoft.AspNetCore.Http.IFormFile? file,
            string fieldName)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException(
                    fieldName + " is required.");
            }

            if (file.Length > MaxFileSize)
            {
                throw new ArgumentException(
                    fieldName + " cannot exceed 5 MB.");
            }

            var extension =
                Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!AllowedExtensions.Contains(extension))
            {
                throw new ArgumentException(
                    fieldName + " must be JPG, JPEG, PNG or WEBP.");
            }

            if (file.ContentType == null ||
                !file.ContentType.StartsWith(
                    "image/", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    fieldName + " must be an image.");
            }
        }

        // SAVE IMAGE
        private async Task<string> SaveImageAsync(
            Microsoft.AspNetCore.Http.IFormFile file)
        {
            var uploadFolder = Path.Combine(
                _environment.ContentRootPath,
                "wwwroot",
                "uploads",
                "deliveries");

            Directory.CreateDirectory(uploadFolder);

            var extension =
                Path.GetExtension(file.FileName).ToLowerInvariant();

            var fileName = Guid.NewGuid() + extension;

            var fullPath = Path.Combine(uploadFolder, fileName);

            using (var stream = new FileStream(
                fullPath, FileMode.CreateNew))
            {
                await file.CopyToAsync(stream);
            }

            return "/uploads/deliveries/" + fileName;
        }

        // DELETE UPLOADED FILE ON FAILURE
        private void DeleteUploadedFile(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return;

            var fileName = Path.GetFileName(relativePath);

            var fullPath = Path.Combine(
                _environment.ContentRootPath,
                "wwwroot",
                "uploads",
                "deliveries",
                fileName);

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}

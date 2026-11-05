using Microsoft.AspNetCore.Http;

namespace ConstructionManagement.BLL.DTOs
{
    public class CreateMaterialDeliveryRequest
    {
        public long DemandItemId { get; set; }

        public decimal ReceivedQuantity { get; set; }

        public string? ChallanNumber { get; set; }

        public IFormFile ChallanPhoto { get; set; } = null!;

        public IFormFile? InvoicePhoto { get; set; }

        public string? InvoiceNumber { get; set; }

        public decimal? UnitRate { get; set; }

        public string? Remarks { get; set; }
    }
}
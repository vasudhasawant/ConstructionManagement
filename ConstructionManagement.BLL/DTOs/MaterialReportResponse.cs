namespace ConstructionManagement.BLL.DTOs.Dashboard
{
    public class MaterialReportResponse
    {
        public long DemandId { get; set; }
        public int SiteId { get; set; }
        public string SiteName { get; set; } = string.Empty;
        public string MaterialName { get; set; } = string.Empty;
        public decimal RequestedQuantity { get; set; }
        public decimal? ApprovedQuantity { get; set; }
        public decimal TotalReceivedQuantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public string DemandStatus { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}
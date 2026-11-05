namespace ConstructionManagement.BLL.DTOs.Dashboard
{
    public class DashboardSummaryResponse
    {
        public int TotalContractors { get; set; }
        public int PresentToday { get; set; }
        public int PendingApprovals { get; set; }
        public int TotalMaterialDemands { get; set; }
        public int PendingMaterialDemands { get; set; }
        public decimal TotalMaterialAmount { get; set; }
    }
}
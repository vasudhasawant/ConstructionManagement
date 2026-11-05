namespace ConstructionManagement.BLL.DTOs.Dashboard
{
    public class AttendanceReportResponse
    {
        public long SubmissionId { get; set; }
        public int SiteId { get; set; }
        public string SiteName { get; set; } = string.Empty;
        public string ContractorName { get; set; } = string.Empty;
        public DateOnly WorkDate { get; set; }
        public int DeclaredLabourCount { get; set; }
        public int? VerifiedLabourCount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? WorkDescription { get; set; }
    }
}
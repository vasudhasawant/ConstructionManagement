using Microsoft.AspNetCore.Http;

namespace ConstructionManagement.BLL.DTOs
{
    public class CreateDailyWorkSubmissionRequest
    {
        public int SiteId { get; set; }

        public int ContractorId { get; set; }

        public DateOnly WorkDate { get; set; }

        public IFormFile? AttendanceSelfie { get; set; }

        public IFormFile? WorkPhoto { get; set; }

        public int DeclaredLabourCount { get; set; }

        public string WorkDescription { get; set; } = string.Empty;

        public string? Latitude { get; set; }

        public string? Longitude { get; set; }
    }
}
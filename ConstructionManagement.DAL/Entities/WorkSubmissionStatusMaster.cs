namespace ConstructionManagement.DAL.Entities
{
    public partial class WorkSubmissionStatusMaster
    {
        public int StatusId { get; set; }

        public string StatusName { get; set; } = null!;
        public virtual ICollection<DailyWorkSubmission> DailyWorkSubmissions { get; set; }
    = new List<DailyWorkSubmission>();
    }
}
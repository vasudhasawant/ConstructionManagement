using ConstructionManagement.BLL.DTOs;
using ConstructionManagement.DAL.Context;
using ConstructionManagement.DAL.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace ConstructionManagement.BLL.Services
{
    public class DailyWorkSubmissionService
    {
        private readonly ConstructionDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public DailyWorkSubmissionService(
            ConstructionDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<bool> CreateSubmissionAsync(
            CreateDailyWorkSubmissionRequest request)
        {
            // Check site
            var site = await _context.ConstructionSites
                .FirstOrDefaultAsync(x =>
                    x.SiteId == request.SiteId && x.IsActive);

            if (site == null)
                throw new Exception("Site not found.");

            // Check contractor
            var contractor = await _context.Contractors
                .FirstOrDefaultAsync(x =>
                    x.ContractorId == request.ContractorId &&
                    x.IsActive);

            if (contractor == null)
                throw new Exception("Contractor not found.");

            // Check duplicate attendance
            var existing = await _context.DailyWorkSubmissions
       .AnyAsync(x =>
           x.SiteId == request.SiteId &&
           x.ContractorId == request.ContractorId &&
           x.WorkDate == request.WorkDate);

            if (existing)
                throw new Exception("Attendance already submitted for this date.");

            // Validate photos
            if (request.AttendanceSelfie == null ||
                request.WorkPhoto == null)
                throw new Exception("Attendance selfie and work photo are required.");

            // Create upload folder
            var uploadFolder = Path.Combine(
                _environment.WebRootPath ?? "wwwroot",
                "uploads",
                "attendance");

            Directory.CreateDirectory(uploadFolder);

            // Generate unique file names
            var selfieFileName = Guid.NewGuid() +
                Path.GetExtension(request.AttendanceSelfie.FileName);

            var workFileName = Guid.NewGuid() +
                Path.GetExtension(request.WorkPhoto.FileName);

            var selfiePath = Path.Combine(uploadFolder, selfieFileName);
            var workPath = Path.Combine(uploadFolder, workFileName);

            // Save photos
            using (var stream = new FileStream(selfiePath, FileMode.Create))
            {
                await request.AttendanceSelfie.CopyToAsync(stream);
            }

            using (var stream = new FileStream(workPath, FileMode.Create))
            {
                await request.WorkPhoto.CopyToAsync(stream);
            }

            // Save attendance
            var submission = new DailyWorkSubmission
            {
                SiteId = request.SiteId,
                ContractorId = request.ContractorId,
                WorkDate = request.WorkDate,
                AttendanceSelfiePath = "/uploads/attendance/" + selfieFileName,
                WorkPhotoPath = "/uploads/attendance/" + workFileName,
                DeclaredLabourCount = request.DeclaredLabourCount,
                WorkDescription = request.WorkDescription,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                SubmittedAt = DateTime.UtcNow,
                StatusId = 1
               // Status = "PendingSupervisor"
            };

            _context.DailyWorkSubmissions.Add(submission);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var errorMessage = ex.InnerException?.Message
                                   ?? ex.Message;

                throw new Exception(errorMessage);
            }
            return true;
        }

        public async Task<List<object>> GetAllSubmissionsAsync()
        {
            var submissions = await _context.DailyWorkSubmissions
                .AsNoTracking()
                .Select(x => new
                {
                    x.SubmissionId,
                    x.SiteId,
                    x.ContractorId,
                    x.WorkDate,
                    x.AttendanceSelfiePath,
                    x.WorkPhotoPath,
                    x.DeclaredLabourCount,
                    x.VerifiedLabourCount,
                    x.WorkDescription,
                    x.Latitude,
                    x.Longitude,
                    x.StatusNavigation.StatusName,
                    //x.Status,
                    x.SubmittedAt
                })
                .ToListAsync();

            return submissions.Cast<object>().ToList();
        }

        public async Task<object?> GetSubmissionByIdAsync(int id)
        {
            var submission = await _context.DailyWorkSubmissions
                .AsNoTracking()
                .Where(x => x.SubmissionId == id)
                .Select(x => new
                {
                    x.SubmissionId,
                    x.SiteId,
                    x.ContractorId,
                    x.WorkDate,
                    x.AttendanceSelfiePath,
                    x.WorkPhotoPath,
                    x.DeclaredLabourCount,
                    x.VerifiedLabourCount,
                    x.WorkDescription,
                    x.Latitude,
                    x.Longitude,
                    //x.Status,
                    x.StatusNavigation.StatusName,
                    x.SubmittedAt
                })
                .FirstOrDefaultAsync();

            return submission;
        }

        public async Task<List<object>> GetSubmissionsBySiteAsync(int siteId)
        {
            var submissions = await _context.DailyWorkSubmissions
                .AsNoTracking()
                .Where(x => x.SiteId == siteId)
                .Select(x => new
                {
                    x.SubmissionId,
                    x.SiteId,
                    x.ContractorId,
                    x.WorkDate,
                    x.AttendanceSelfiePath,
                    x.WorkPhotoPath,
                    x.DeclaredLabourCount,
                    x.VerifiedLabourCount,
                    x.WorkDescription,
                    x.Latitude,
                    x.Longitude,
                    //x.Status,
                    x.StatusNavigation.StatusName,
                    x.SubmittedAt
                })
                .ToListAsync();

            return submissions.Cast<object>().ToList();
        }
    }
}
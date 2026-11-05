using ConstructionManagement.BLL.DTOs;
using ConstructionManagement.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstructionManagement.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DailyWorkSubmissionsController : ControllerBase
    {
        private readonly DailyWorkSubmissionService _service;

        public DailyWorkSubmissionsController(
            DailyWorkSubmissionService service)
        {
            _service = service;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateSubmission(
            [FromForm] CreateDailyWorkSubmissionRequest request)
        {
            try
            {
                await _service.CreateSubmissionAsync(request);

                return Ok(new
                {
                    message = "Daily work and attendance submitted successfully."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSubmissions()
        {
            try
            {
                var submissions = await _service.GetAllSubmissionsAsync();

                return Ok(submissions);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubmissionById(int id)
        {
            try
            {
                var submission = await _service.GetSubmissionByIdAsync(id);

                if (submission == null)
                {
                    return NotFound(new
                    {
                        message = "Attendance submission not found."
                    });
                }

                return Ok(submission);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet("site/{siteId}")]
        public async Task<IActionResult> GetSubmissionsBySite(int siteId)
        {
            try
            {
                var submissions = await _service.GetSubmissionsBySiteAsync(siteId);

                return Ok(submissions);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
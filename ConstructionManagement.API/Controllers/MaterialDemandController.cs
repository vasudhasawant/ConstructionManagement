using ConstructionManagement.BLL.DTOs;
using ConstructionManagement.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ConstructionManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Supervisor,Builder")]
    public class MaterialDemandController : ControllerBase
    {
        private readonly MaterialDemandService _materialDemandService;

        public MaterialDemandController(
            MaterialDemandService materialDemandService)
        {
            _materialDemandService = materialDemandService;
        }

        // Create Material Demand
        [HttpPost]
        public async Task<IActionResult> CreateDemand(
            [FromBody] CreateMaterialDemandRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null ||
                !int.TryParse(userIdClaim.Value, out int requestedBy))
            {
                return Unauthorized("Invalid user information.");
            }

            try
            {
                var demandId =
                    await _materialDemandService.CreateDemandAsync(
                        requestedBy, request);

                return Ok(new
                {
                    message = "Material demand created successfully.",
                    demandId
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        // Get All Material Demands
        [HttpGet]
        public async Task<IActionResult> GetAllDemands()
        {
            var demands = await _materialDemandService.GetAllDemandsAsync();

            return Ok(demands);
        }

        // Get Material Demand By ID
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetDemandById(long id)
        {
            var demand = await _materialDemandService.GetDemandByIdAsync(id);

            if (demand == null)
                return NotFound(new
                {
                    message = "Material demand not found."
                });

            return Ok(demand);
        }


        // Update Material Demand Status
        [HttpPut("{id:long}/status")]
        [Authorize(Roles = "Builder")]
        public async Task<IActionResult> UpdateDemandStatus(
            long id,
            [FromBody] UpdateMaterialDemandStatusRequest request)
        {
            try
            {
                var result = await _materialDemandService
                    .UpdateDemandStatusAsync(id, request);

                if (!result)
                {
                    return NotFound(new
                    {
                        message = "Material demand not found."
                    });
                }

                return Ok(new
                {
                    message = "Material demand status updated successfully."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
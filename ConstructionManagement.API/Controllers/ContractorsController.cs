using ConstructionManagement.BLL.DTOs;
using ConstructionManagement.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstructionManagement.API.Controllers
{
    [Authorize(Roles = "Admin,Builder")]
    [ApiController]
    [Route("api/[controller]")]
    public class ContractorsController : ControllerBase
    {
        private readonly ContractorService _service;

        public ContractorsController(ContractorService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateContractor(
            [FromBody] CreateContractorRequest request)
        {
            try
            {
                await _service.CreateContractorAsync(request);

                return Ok(new
                {
                    message = "Contractor created successfully."
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
        public async Task<IActionResult> GetAllContractors()
        {
            try
            {
                var contractors = await _service.GetAllContractorsAsync();

                return Ok(contractors);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContractor(
        int id,
        [FromBody] UpdateContractorRequest request)
        {
            try
            {
                var result = await _service.UpdateContractorAsync(id, request);

                if (!result)
                {
                    return NotFound(new
                    {
                        message = "Contractor not found."
                    });
                }

                return Ok(new
                {
                    message = "Contractor updated successfully."
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeactivateContractor(int id)
        {
            try
            {
                var result = await _service.DeactivateContractorAsync(id);

                if (!result)
                {
                    return NotFound(new
                    {
                        message = "Contractor not found or already inactive."
                    });
                }

                return Ok(new
                {
                    message = "Contractor deactivated successfully."
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
    }
}
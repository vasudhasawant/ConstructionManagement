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
    public class MaterialDeliveryController : ControllerBase
    {
        private readonly MaterialDeliveryService _service;

        public MaterialDeliveryController(
            MaterialDeliveryService service)
        {
            _service = service;
        }

        // 1. CREATE MATERIAL DELIVERY
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateDelivery(
            [FromForm] CreateMaterialDeliveryRequest request)
        {
            var userIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null ||
                !int.TryParse(userIdClaim.Value, out int receivedBy))
            {
                return Unauthorized(
                    "Invalid user information.");
            }

            try
            {
                var deliveryId =
                    await _service.CreateDeliveryAsync(
                        receivedBy, request);

                return Ok(new
                {
                    message = "Material delivery recorded successfully.",
                    deliveryId
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // 2. GET ALL MATERIAL DELIVERIES
        [HttpGet]
        public async Task<IActionResult> GetAllDeliveries()
        {
            var deliveries =
                await _service.GetAllDeliveriesAsync();

            return Ok(deliveries);
        }

        // 3. GET MATERIAL DELIVERY BY ID
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetDeliveryById(long id)
        {
            var delivery =
                await _service.GetDeliveryByIdAsync(id);

            if (delivery == null)
            {
                return NotFound(new
                {
                    message = "Material delivery not found."
                });
            }

            return Ok(delivery);
        }
    }
}
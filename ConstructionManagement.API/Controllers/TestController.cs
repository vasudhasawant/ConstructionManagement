using ConstructionManagement.BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConstructionManagement.API.Controllers;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    [HttpGet("generate-hash")]
    public IActionResult GenerateHash()
    {
        var hash = PasswordHelper.HashPassword("Builder@123");

        return Ok(new { passwordHash = hash });
    }
}
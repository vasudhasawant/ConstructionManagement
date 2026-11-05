using ConstructionManagement.BLL.DTOs;
using ConstructionManagement.BLL.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace ConstructionManagement.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Phone) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new
            {
                message = "Phone and password are required."
            });
        }

        var result = await _authService.LoginAsync(request);

        if (result == null)
        {
            return Unauthorized(new
            {
                message = "Invalid phone number or password."
            });
        }

        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("register")]
    public async Task<IActionResult> Register(
    [FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName) ||
            string.IsNullOrWhiteSpace(request.Phone) ||
            string.IsNullOrWhiteSpace(request.Password) ||
            string.IsNullOrWhiteSpace(request.Role))
        {
            return BadRequest(new
            {
                message = "FullName, Phone, Password and Role are required."
            });
        }

        if (request.Password.Length < 8)
        {
            return BadRequest(new
            {
                message = "Password must contain at least 8 characters."
            });
        }

        try
        {
            var result = await _authService.RegisterAsync(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
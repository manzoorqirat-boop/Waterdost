using Microsoft.AspNetCore.Mvc;
using WaterApp.Application.DTOs;
using WaterApp.Application.Interfaces;

namespace WaterApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        try
        {
            var result = await _authService.RegisterAsync(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    // Protected admin-creation endpoint.
    // Requires header: X-Admin-Key: <value of ADMIN_SEED_KEY env var>
    // Use this once to seed your first Admin user, then treat the key as a secret
    // (rotate it in Railway if you suspect it's leaked).
    [HttpPost("register-admin")]
    public async Task<ActionResult<AuthResponse>> RegisterAdmin(
        RegisterRequest request,
        [FromHeader(Name = "X-Admin-Key")] string? adminKey)
    {
        var expectedKey = Environment.GetEnvironmentVariable("ADMIN_SEED_KEY");

        if (string.IsNullOrEmpty(expectedKey))
            return StatusCode(503, new { message = "ADMIN_SEED_KEY is not configured on the server." });

        if (string.IsNullOrEmpty(adminKey) || adminKey != expectedKey)
            return Unauthorized(new { message = "Invalid or missing admin key." });

        try
        {
            var result = await _authService.RegisterAdminAsync(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        try
        {
            var result = await _authService.LoginAsync(request);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}

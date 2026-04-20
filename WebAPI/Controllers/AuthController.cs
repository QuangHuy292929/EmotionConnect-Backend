using System.Security.Claims;
using System.Text.Json;                    // ← fix lỗi JsonSerializer
using Application.DTOs.Auth;
using Application.Interfaces.IServices;
using Infracstructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration; // ← fix lỗi _config

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IConfiguration _config;

    public AuthController(IAuthService authService, IConfiguration config) 
    {
        _authService = authService;
        _config = config; 
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.RegisterAsync(request, cancellationToken);
        return Ok(new { data = response, message = "success" }); 
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.LoginAsync(request, cancellationToken);

        Response.Cookies.Append("token", response.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddHours(1)
        });

        return Ok(new { data = response, message = "success" });
    }

    [HttpGet("google-login")]
    [AllowAnonymous]
    public IActionResult GoogleLogin()
    {
        var clientId = _config["Google:ClientId"];
        var redirectUri = _config["Google:RedirectUri"];
        var state = Guid.NewGuid().ToString();

        var url = "https://accounts.google.com/o/oauth2/v2/auth" +
                  $"?client_id={clientId}" +
                  $"&redirect_uri={Uri.EscapeDataString(redirectUri!)}" +
                  "&response_type=code" +
                  "&scope=openid%20email%20profile" +
                  $"&state={state}";

        return Redirect(url);
    }

    [HttpGet("google-callback")]
    [AllowAnonymous]
    public async Task<IActionResult> GoogleCallback(
    [FromQuery] string code,
    [FromQuery] string state,
    CancellationToken cancellationToken)
    {
        try
        {
            var response = await _authService.GoogleLoginAsync(code, cancellationToken);

            Response.Cookies.Append("token", response.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });

            return Redirect("http://localhost:5174/auth/callback");
        }
        catch (Exception ex)
        {
            return Redirect($"http://localhost:5174/login?error={Uri.EscapeDataString(ex.Message)}");
        }
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserSummaryDto>> Me(CancellationToken cancellationToken)
    {
        var userId = User.GetCurrentUserId();
        var user = await _authService.GetCurrentUserAsync(userId, cancellationToken);

        if (user is null)
            return NotFound(new { message = "User not found." });

        return Ok(user);
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("token");
        return Ok(new { message = "Logged out" });
    }
}
using Application.DTOs.Auth;
using Application.Interfaces.IServices;
using Infracstructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IConfiguration _config;
    private readonly IWebHostEnvironment _environment;

    public AuthController(
        IAuthService authService,
        IConfiguration config,
        IWebHostEnvironment environment)
    {
        _authService = authService;
        _config = config;
        _environment = environment;
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
        AppendAuthCookie(response.AccessToken, response.ExpiresAtUtc);

        return Ok(new { data = response, message = "success" });
    }

    [HttpGet("google-login")]
    [AllowAnonymous]
    public IActionResult GoogleLogin()
    {
        var clientId = _config["Google:ClientId"];
        var redirectUri = _config["Google:RedirectUri"];
        var state = Guid.NewGuid().ToString("N");

        Response.Cookies.Append("google_oauth_state", state, new CookieOptions
        {
            HttpOnly = true,
            Secure = !_environment.IsDevelopment(),
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddMinutes(5),
            IsEssential = true
        });

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
            var expectedState = Request.Cookies["google_oauth_state"];
            if (string.IsNullOrWhiteSpace(expectedState) || expectedState != state)
            {
                return Unauthorized(new { message = "Invalid OAuth state." });
            }

            Response.Cookies.Delete("google_oauth_state");

            var response = await _authService.GoogleLoginAsync(code, cancellationToken);
            AppendAuthCookie(response.AccessToken, response.ExpiresAtUtc);

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
        Console.WriteLine($"=== UserId from token: {userId} ==="); 

        var user = await _authService.GetCurrentUserAsync(userId, cancellationToken);

        if (user is null)
        {
            return NotFound(new { message = "User not found." });
        }

        return Ok(user);
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        // Phải set đúng các options như lúc tạo cookie thì browser mới xoá được.
        // Chỉ gọi Delete() không đủ — browser bỏ qua nếu Path/SameSite không khớp.
        var expiredOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = !_environment.IsDevelopment(),
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(-1), // quá khứ → browser xoá ngay
            IsEssential = true
        };

        Response.Cookies.Append("token", string.Empty, expiredOptions);
        Response.Cookies.Append("google_oauth_state", string.Empty, expiredOptions);

        return Ok(new { message = "Logged out" });
    }

    private void AppendAuthCookie(string token, DateTime expiresAtUtc)
    {
        Response.Cookies.Append("token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = !_environment.IsDevelopment(),
            SameSite = SameSiteMode.Lax,
            Expires = new DateTimeOffset(expiresAtUtc),
            IsEssential = true
        });
    }
}

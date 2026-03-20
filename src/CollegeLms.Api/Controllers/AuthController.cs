using System.Security.Claims;
using CollegeLms.Api.DTOs.Auth;
using CollegeLms.Api.Services;
using CollegeLms.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeLms.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly JwtTokenService _jwtService;

    public AuthController(AuthService authService, JwtTokenService jwtService)
    {
        _authService = authService;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        if (result == null)
            return Unauthorized(new { message = "Неверный email или пароль" });

        // Get the refresh token from DB (last created for this user)
        SetRefreshTokenCookie(await GetLastRefreshToken(request.Email));

        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized(new { message = "Refresh token не найден" });

        var (response, newRefreshToken) = await _authService.RefreshAsync(refreshToken);
        if (response == null)
            return Unauthorized(new { message = "Невалидный refresh token" });

        SetRefreshTokenCookie(newRefreshToken!);
        return Ok(response);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var userId = GetCurrentUserId();
        await _authService.LogoutAsync(userId);

        Response.Cookies.Delete("refreshToken");
        return Ok(new { message = "Вы вышли из системы" });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = GetCurrentUserId();
        var user = await _authService.GetCurrentUserAsync(userId);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("register-student")]
    public async Task<IActionResult> RegisterStudent([FromBody] RegisterRequest request)
    {
        var user = await _authService.RegisterAsync(request, UserRole.Student);
        if (user == null)
            return Conflict(new { message = "Пользователь с таким email уже существует" });

        return CreatedAtAction(nameof(GetCurrentUser), user);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("register-teacher")]
    public async Task<IActionResult> RegisterTeacher([FromBody] RegisterRequest request)
    {
        var user = await _authService.RegisterAsync(request, UserRole.Teacher);
        if (user == null)
            return Conflict(new { message = "Пользователь с таким email уже существует" });

        return CreatedAtAction(nameof(GetCurrentUser), user);
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });
    }

    private Guid GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return Guid.Parse(claim!.Value);
    }

    private async Task<string> GetLastRefreshToken(string email)
    {
        // This is a workaround — in production, AuthService.LoginAsync should return the refresh token
        using var scope = HttpContext.RequestServices.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CollegeLms.Infrastructure.Data.AppDbContext>();
        var user = await db.Users.FirstAsync(u => u.Email == email);
        var token = await db.RefreshTokens
            .Where(rt => rt.UserId == user.Id && !rt.IsRevoked)
            .OrderByDescending(rt => rt.CreatedAt)
            .FirstAsync();
        return token.Token;
    }
}

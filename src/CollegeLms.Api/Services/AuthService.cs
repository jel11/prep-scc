using CollegeLms.Api.DTOs.Auth;
using CollegeLms.Domain.Entities;
using CollegeLms.Domain.Enums;
using CollegeLms.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CollegeLms.Api.Services;

public class AuthService
{
    private readonly AppDbContext _db;
    private readonly JwtTokenService _jwtService;

    public AuthService(AppDbContext db, JwtTokenService jwtService)
    {
        _db = db;
        _jwtService = jwtService;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        var (accessToken, expiresAt) = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        _db.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();

        return new LoginResponse
        {
            AccessToken = accessToken,
            ExpiresAt = expiresAt,
            User = MapToDto(user)
        };
    }

    public string? GetRefreshTokenFromLogin(LoginRequest request)
    {
        return null; // Refresh token is returned via cookie, handled in controller
    }

    public async Task<(LoginResponse? response, string? refreshToken)> RefreshAsync(string refreshTokenValue)
    {
        var storedToken = await _db.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshTokenValue && !rt.IsRevoked);

        if (storedToken == null || storedToken.ExpiresAt < DateTime.UtcNow)
            return (null, null);

        // Revoke old token
        storedToken.IsRevoked = true;

        // Generate new tokens
        var (accessToken, expiresAt) = _jwtService.GenerateAccessToken(storedToken.User);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        _db.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = storedToken.UserId,
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();

        var response = new LoginResponse
        {
            AccessToken = accessToken,
            ExpiresAt = expiresAt,
            User = MapToDto(storedToken.User)
        };

        return (response, newRefreshToken);
    }

    public async Task LogoutAsync(Guid userId)
    {
        var tokens = await _db.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync();

        foreach (var token in tokens)
            token.IsRevoked = true;

        await _db.SaveChangesAsync();
    }

    public async Task<UserDto?> RegisterAsync(RegisterRequest request, UserRole role)
    {
        if (await _db.Users.AnyAsync(u => u.Email == request.Email))
            return null;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, 12),
            FullName = request.FullName,
            Role = role,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return MapToDto(user);
    }

    public async Task<UserDto?> GetCurrentUserAsync(Guid userId)
    {
        var user = await _db.Users.FindAsync(userId);
        return user == null ? null : MapToDto(user);
    }

    private static UserDto MapToDto(User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        FullName = user.FullName,
        Role = user.Role.ToString(),
        PhotoUrl = user.PhotoPath
    };
}

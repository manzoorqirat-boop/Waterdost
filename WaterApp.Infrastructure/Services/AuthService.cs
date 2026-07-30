using WaterApp.Application.DTOs;
using WaterApp.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using WaterApp.Domain.Entities;
using WaterApp.Domain.Enums;
using WaterApp.Infrastructure.Data;

namespace WaterApp.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokenService;

    public AuthService(AppDbContext db, IPasswordHasher hasher, ITokenService tokenService)
    {
        _db = db;
        _hasher = hasher;
        _tokenService = tokenService;
    }

    // Public self-registration. Admin accounts can NEVER be created through this path,
    // even if a caller sends Role: 2 in the request body.
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (request.Role == UserRole.Admin)
            throw new InvalidOperationException("Admin accounts cannot be created via public registration.");

        return await CreateUserAsync(request, request.Role);
    }

    // Only reachable via the key-protected /api/auth/register-admin endpoint.
    // Forces Admin role regardless of what the caller passed in Role.
    public async Task<AuthResponse> RegisterAdminAsync(RegisterRequest request)
    {
        return await CreateUserAsync(request, UserRole.Admin);
    }

    private async Task<AuthResponse> CreateUserAsync(RegisterRequest request, UserRole role)
    {
        var exists = await _db.Users.AnyAsync(u => u.Phone == request.Phone);
        if (exists)
            throw new InvalidOperationException("A user with this phone number already exists.");

        var user = new User
        {
            Name = request.Name,
            Phone = request.Phone,
            Email = request.Email,
            PasswordHash = _hasher.Hash(request.Password),
            Role = role
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var access = _tokenService.GenerateAccessToken(user);
        var refresh = _tokenService.GenerateRefreshToken();

        return new AuthResponse(user.Id, user.Name, user.Role, access, refresh);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Phone == request.Phone)
            ?? throw new UnauthorizedAccessException("Invalid phone number or password.");

        if (!_hasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid phone number or password.");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("This account has been deactivated. Please contact support.");

        var access = _tokenService.GenerateAccessToken(user);
        var refresh = _tokenService.GenerateRefreshToken();

        return new AuthResponse(user.Id, user.Name, user.Role, access, refresh);
    }
}

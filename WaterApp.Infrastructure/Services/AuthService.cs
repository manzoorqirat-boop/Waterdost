
    public AuthService(AppDbContext db, IPasswordHasher hasher, ITokenService tokenService)
    {
        _db = db;
        _hasher = hasher;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
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
            Role = request.Role
        };

using Microsoft.EntityFrameworkCore;
using WaterApp.Application.DTOs;
using WaterApp.Application.Interfaces;
using WaterApp.Domain.Entities;
using WaterApp.Infrastructure.Data;

namespace WaterApp.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokenService;
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

        var access = _tokenService.GenerateAccessToken(user);
        var refresh = _tokenService.GenerateRefreshToken();

        return new AuthResponse(user.Id, user.Name, user.Role, access, refresh);
    }
}

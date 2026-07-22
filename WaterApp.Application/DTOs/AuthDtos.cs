using WaterApp.Domain.Enums;

namespace WaterApp.Application.DTOs;

public record RegisterRequest(string Name, string Phone, string? Email, string Password, UserRole Role);

public record LoginRequest(string Phone, string Password);

public record AuthResponse(Guid UserId, string Name, UserRole Role, string AccessToken, string RefreshToken);

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WaterApp.Application.Interfaces;
using WaterApp.Infrastructure.Data;
using WaterApp.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ---- Database ----
// Railway injects DATABASE_URL in URI form (postgresql://user:pass@host:port/db).
// Npgsql needs keyword=value form (Host=...;Username=...;Password=...;Database=...),
// so convert it if a URI-style value is present. Falls back to appsettings locally.
var rawConnectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

var connectionString = ConvertToNpgsqlConnectionString(rawConnectionString);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

static string? ConvertToNpgsqlConnectionString(string? raw)
{
    if (string.IsNullOrWhiteSpace(raw))
        return raw;

    // Already in keyword=value format (e.g. "Host=...;Username=...").
    if (!raw.Contains("://"))
        return raw;

    var uri = new Uri(raw);
    var userInfo = uri.UserInfo.Split(':', 2);
    var username = Uri.UnescapeDataString(userInfo[0]);
    var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty;
    var database = uri.AbsolutePath.TrimStart('/');
    var port = uri.Port == -1 ? 5432 : uri.Port;

    return $"Host={uri.Host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
}

// ---- DI ----
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

// ---- JWT Auth ----
var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? throw new InvalidOperationException("Jwt:Secret is not configured.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
    };
});

builder.Services.AddAuthorization();

// ---- Controllers / Swagger ----
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ---- CORS (open for now; restrict to your app's origin in production) ----
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy
            .WithOrigins(
                "https://waterdost-frontend-production.up.railway.app",
                "http://localhost:8081", // Expo web dev server
                "http://localhost:19006" // legacy Expo web dev port
            )
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Swagger is normally dev-only. Set ENABLE_SWAGGER=true in Railway to turn it on
// in production temporarily (e.g. to seed data via Try It Out), then remove the
// env var afterward to lock it back down.
var swaggerEnabled = app.Environment.IsDevelopment()
    || Environment.GetEnvironmentVariable("ENABLE_SWAGGER") == "true";

if (swaggerEnabled)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// No EF Core migrations exist for this project yet, so create the schema
// directly from the current model on startup. Fine for now; if you later
// add real migrations, remove this and call db.Database.Migrate() instead.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();
}

app.Run();

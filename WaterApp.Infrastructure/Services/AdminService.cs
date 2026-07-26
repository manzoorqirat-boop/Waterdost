using Microsoft.EntityFrameworkCore;
using WaterApp.Application.DTOs;
using WaterApp.Application.Interfaces;
using WaterApp.Domain.Enums;
using WaterApp.Infrastructure.Data;

namespace WaterApp.Infrastructure.Services;

public class AdminService : IAdminService
{
    private readonly AppDbContext _db;

    public AdminService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<AdminStatsResponse> GetStatsAsync()
    {
        var totalUsers = await _db.Users.CountAsync();
        var totalBuyers = await _db.Users.CountAsync(u => u.Role == UserRole.Buyer);
        var totalSellers = await _db.Users.CountAsync(u => u.Role == UserRole.Seller);
        var totalAdmins = await _db.Users.CountAsync(u => u.Role == UserRole.Admin);

        var pendingSellers = await _db.Sellers.CountAsync(s => s.Status == SellerStatus.Pending);
        var approvedSellers = await _db.Sellers.CountAsync(s => s.Status == SellerStatus.Approved);

        var totalOrders = await _db.Orders.CountAsync();
        var totalRevenue = await _db.Orders
            .Where(o => o.PaymentStatus == PaymentStatus.Success || o.PaymentStatus == PaymentStatus.CollectedInCash)
            .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

        return new AdminStatsResponse(
            totalUsers,
            totalBuyers,
            totalSellers,
            totalAdmins,
            pendingSellers,
            approvedSellers,
            totalOrders,
            totalRevenue
        );
    }

    public async Task<List<AdminSellerResponse>> GetSellersAsync(string? status)
    {
        var query = _db.Sellers.Include(s => s.User).AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<SellerStatus>(status, true, out var parsedStatus))
                throw new ArgumentException($"Unknown seller status '{status}'.");
            query = query.Where(s => s.Status == parsedStatus);
        }

        var sellers = await query.OrderByDescending(s => s.CreatedAt).ToListAsync();

        return sellers.Select(s => new AdminSellerResponse(
            s.Id,
            s.UserId,
            s.User?.Name ?? "",
            s.User?.Phone ?? "",
            s.User?.Email,
            s.CompanyName,
            s.Status.ToString(),
            s.CreatedAt
        )).ToList();
    }

    public async Task<AdminSellerResponse> UpdateSellerStatusAsync(Guid sellerId, string status)
    {
        if (!Enum.TryParse<SellerStatus>(status, true, out var parsedStatus))
            throw new ArgumentException($"Unknown seller status '{status}'.");

        var seller = await _db.Sellers.Include(s => s.User).FirstOrDefaultAsync(s => s.Id == sellerId)
            ?? throw new KeyNotFoundException("Seller not found.");

        seller.Status = parsedStatus;
        await _db.SaveChangesAsync();

        return new AdminSellerResponse(
            seller.Id,
            seller.UserId,
            seller.User?.Name ?? "",
            seller.User?.Phone ?? "",
            seller.User?.Email,
            seller.CompanyName,
            seller.Status.ToString(),
            seller.CreatedAt
        );
    }
}

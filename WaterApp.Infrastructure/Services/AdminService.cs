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
}

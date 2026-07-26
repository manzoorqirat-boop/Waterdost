using WaterApp.Application.DTOs;

namespace WaterApp.Application.Interfaces;

public interface IAdminService
{
    Task<AdminStatsResponse> GetStatsAsync();
    Task<List<AdminSellerResponse>> GetSellersAsync(string? status);
    Task<AdminSellerResponse> UpdateSellerStatusAsync(Guid sellerId, string status);
}

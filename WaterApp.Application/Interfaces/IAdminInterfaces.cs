using WaterApp.Application.DTOs;

namespace WaterApp.Application.Interfaces;

public interface IAdminService
{
    Task<AdminStatsResponse> GetStatsAsync();
}

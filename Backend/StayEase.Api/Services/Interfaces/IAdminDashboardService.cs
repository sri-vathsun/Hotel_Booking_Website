using StayEase.Api.DTOs.Admin;

namespace StayEase.Api.Services.Interfaces
{
    public interface IAdminDashboardService
    {
        Task<DashboardStatsDto> GetDashboardStatsAsync();
    }
}
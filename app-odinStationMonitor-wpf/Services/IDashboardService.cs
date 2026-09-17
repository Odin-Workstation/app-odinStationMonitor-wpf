using TacoStationMonitor.Models;

namespace TacoStationMonitor.Service
{
    public interface IDashboardService
    {
        Task<DashboardState?> GetDashboardData();
        Task<DashboardState?> GetDashboardDataAsync(int partId);
    }
}
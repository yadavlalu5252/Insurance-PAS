
using DashboardNotification.Application.DTO;

namespace DashboardNotification.Application.Interface
{
    public interface IDashboardMetrics
    {
        Task<DashboardMetrics> GetDashboardMetrics(string token);
    }
}

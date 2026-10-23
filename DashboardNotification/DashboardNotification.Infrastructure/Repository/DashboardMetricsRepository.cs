
using DashboardNotification.Application.DTO;
using DashboardNotification.Application.Interface;
using DashboardNotification.Infrastructure.Data;

namespace DashboardNotification.Infrastructure.Repository
{
    //public class DashboardMetricsRepository : IDashboardMetrics
    //{
    //    private readonly DashboardNotificationDbContext db;

    //    public DashboardMetricsRepository(DashboardNotificationDbContext db)
    //    {
    //        this.db = db;
    //    }

        //public DashboardMetrics GetDashboardMetrics()
        //{
        //    DashboardMetrics data = new DashboardMetrics();

        //    data.TotalUsers = db.Users.Count();

        //    data.ActiveUsers = db.Users.Count(x => x.status == "Active");

        //    data.LockedUsers = db.Users.Count(x => x.status == "Locked");

        //    data.TotalRoles = db.Roles.Count();

        //    return data;
        //}
//    }
}


using DashboardNotification.Api.Wrappers;
using DashboardNotification.Application.DTO;
using DashboardNotification.Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace DashboardNotification.Api.Controllers
{
    [Route("api/v1/dashboard-metrics")]
    [ApiController]
    public class DashboardMetricsController : ControllerBase
    {
       // private readonly IDashboardMetrics service;

        //public DashboardMetricsController(IDashboardMetrics service)
        //{
        //    this.service = service;
        //}

        //[HttpGet]
        //public IActionResult GetDashboardMetrics()
        //{
        //    var data = service.GetDashboardMetrics();

        //    var response = ApiResponse<DashboardMetrics>.SuccessResponse(data);

        //    return Ok(response);
        //}
   }
}

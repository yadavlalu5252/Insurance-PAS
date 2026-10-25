
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
        private readonly IDashboardMetrics service;

        public DashboardMetricsController(IDashboardMetrics service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardMetrics()
        {
            string token = Request.Headers.Authorization.ToString()
                .Replace("Bearer ", "");

            var data = await service.GetDashboardMetrics(token);

            var response = ApiResponse<DashboardMetrics>.SuccessResponse(data);

            return Ok(response);
        }
    }
}

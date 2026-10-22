
using DashboardNotification.Api.Wrappers;
using DashboardNotification.Application.Interface;
using DashboardNotification.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DashboardNotification.Api.Controllers
{
    [Route("api/v1/notification-templates")]
    [ApiController]
    public class NotificationTemplateController : ControllerBase
    {
        private readonly INotificationTemplate service;

        public NotificationTemplateController(INotificationTemplate service)
        {
            this.service = service;
        }

        [HttpPost]
        public IActionResult AddNotificationTemplate(NotificationTemplate model)
        {
            service.AddNotificationTemplate(model);
            var response = ApiResponse<string>.SuccessResponse("Notification template added successfully");
            return Ok(response);
        }

        [HttpGet]
        public IActionResult GetAllNotificationTemplates()
        {
            var data = service.GetAllNotificationTemplates();
            var response = ApiResponse<List<NotificationTemplate>>.SuccessResponse(data);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateNotificationTemplate(
            long id, NotificationTemplate model)
        {
            model.template_id = id;
            service.UpdateNotificationTemplate(model);
            var response = ApiResponse<string>.SuccessResponse("Notification template updated successfully");
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteNotificationTemplate(long id)
        {
            service.DeleteNotificationTemplate(id);
            var response = ApiResponse<string>.SuccessResponse("Notification template deleted successfully");
            return Ok(response);
        }
    }
}

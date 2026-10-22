
using DashboardNotification.Application.Helpers;
using DashboardNotification.Application.Interface;

namespace DashboardNotification.Application.Services
{
    public class NotificationMessageService
    {
        private readonly INotificationTemplate service;
        private readonly NotificationTemplateHelper helper;

        public NotificationMessageService(INotificationTemplate service)
        {
            this.service = service;
            helper = new NotificationTemplateHelper();
        }

        public string? PrepareMessage(
            long templateId,
            Dictionary<string, string> values)
        {
            var template = service.FindNotificationTemplateById(templateId);

            if (template == null || template.body == null)
            {
                return null;
            }

            string message = helper.ReplacePlaceholders(
                template.body,
                values);

            return message;
        }
    }
}

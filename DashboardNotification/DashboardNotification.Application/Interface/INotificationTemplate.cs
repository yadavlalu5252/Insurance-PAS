using DashboardNotification.Domain.Entities;

namespace DashboardNotification.Application.Interface
{
    public interface INotificationTemplate
    {
        void AddNotificationTemplate(NotificationTemplate model);

        List<NotificationTemplate> GetAllNotificationTemplates();

        void DeleteNotificationTemplate(long id);

        NotificationTemplate? FindNotificationTemplateById(long id);

        void UpdateNotificationTemplate(NotificationTemplate model);
    }
}

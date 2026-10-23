
using DashboardNotification.Application.Interface;
using DashboardNotification.Domain.Entities;
using DashboardNotification.Infrastructure.Data;

namespace DashboardNotification.Infrastructure.Repository
{
    public class NotificationTemplateService : INotificationTemplate
    {
        private readonly DashboardNotificationDbContext db;

        public NotificationTemplateService(DashboardNotificationDbContext db)
        {
           this.db = db;
        }

        public void AddNotificationTemplate(NotificationTemplate model)
        {
            db.NotificationTemplates.Add(model);
            db.SaveChanges();
        }

        public List<NotificationTemplate> GetAllNotificationTemplates()
        {
            var data=db.NotificationTemplates.ToList();
            return data;
        }

        public NotificationTemplate? FindNotificationTemplateById(long id)
        {
            var data=db.NotificationTemplates.Find(id);
            return data;
        }


        public void UpdateNotificationTemplate(NotificationTemplate model)
        {
            var data = db.NotificationTemplates.Find(model.template_id);

            if (data != null)
            {
                data.template_name = model.template_name;
                data.notification_type = model.notification_type;
                data.subject = model.subject;
                data.body = model.body;
                data.version_no = data.version_no + 1;
                data.status = model.status;

                db.SaveChanges();
            }
        }


        public void DeleteNotificationTemplate(long id)
        {
            var data= db.NotificationTemplates.Find(id);

            if (data != null)
            {
                db.NotificationTemplates.Remove(data);
                db.SaveChanges();
            }
        }

    }
}

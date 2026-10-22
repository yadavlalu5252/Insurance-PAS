
namespace DashboardNotification.Application.DTO
{
    public class NotificationTemplateDTO
    {
        public long template_id { get; set; }

        public string? template_name { get; set; }

        public string? notification_type { get; set; }

        public string? subject { get; set; }

        public string? body { get; set; }

        public int version_no { get; set; }

        public string? status { get; set; }
    }
}

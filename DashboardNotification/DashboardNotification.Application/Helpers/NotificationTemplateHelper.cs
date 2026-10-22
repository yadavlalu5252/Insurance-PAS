
namespace DashboardNotification.Application.Helpers
{
    public class NotificationTemplateHelper
    {
        public string ReplacePlaceholders(
            string template,
            Dictionary<string, string> values)
        {
            foreach (var item in values)
            {
                template = template.Replace(
                    "{" + item.Key + "}",
                    item.Value);
            }

            return template;
        }
    }
}

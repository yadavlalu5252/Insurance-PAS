namespace PolicyIssuance.Application.DTOs
{
    public class NotificationDTO
    {
        public long policy_id { get; set; }
        public string recipient_email { get; set; } = string.Empty;
        public string subject { get; set; } = string.Empty;
        public string message { get; set; } = string.Empty;
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PremiumSchedule.Domain.Entities
{
    public class PremiumReminder
    {
        [Key]
        public long reminder_id { get; set; }

        [Required]
        public long schedule_id { get; set; }

        [Required]
        public ReminderType reminder_type { get; set; }

        public DateTime reminder_date { get; set; }

        [Required]
        public DeliveryStatus delivery_status { get; set; }
    }
    public enum ReminderType
    {
        SMS,
        Email,
        WhatsApp,
        PushNotification
    }

    public enum DeliveryStatus
    {
        Pending,
        Sent,
        Failed
    }
}

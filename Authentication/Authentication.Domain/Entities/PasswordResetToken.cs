using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Authentication.Domain.Entities
{
    public class PasswordResetToken
    {
        [Key]
        public long reset_token_id { get; set; }

        public long user_id { get; set; }

        public string token_hash { get; set; } = string.Empty;

        public DateTime expires_at { get; set; }

        public DateTime created_at { get; set; } = DateTime.UtcNow;

        public DateTime? used_at { get; set; }
    }
}

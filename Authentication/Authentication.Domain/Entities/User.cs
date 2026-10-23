
using System.ComponentModel.DataAnnotations;

namespace Authentication.Domain.Entities
{
    public class User
    {
        [Key]
        public long user_id { get; set; }

        public long employee_id { get; set; }

        public string username { get; set; } = string.Empty;

        public string password_hash { get; set; } = string.Empty;

        public string? email { get; set; }

        public string? mobile { get; set; }

        public long role_id { get; set; }

        public long branch_id { get; set; }

        public string status { get; set; } = "Active";

        // Two-factor authentication
        public bool two_factor_enabled { get; set; } = false;

        // Store only the protected (encrypted) TOTP secret here.
        public string? two_factor_secret { get; set; }

        // Last accepted TOTP time-step, used to prevent code replay.
        public long? two_factor_last_used_step { get; set; }
    }
}

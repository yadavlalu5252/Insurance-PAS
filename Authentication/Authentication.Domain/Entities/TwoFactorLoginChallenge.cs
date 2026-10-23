
using System.ComponentModel.DataAnnotations;

namespace Authentication.Domain.Entities
{
    public class TwoFactorLoginChallenge
    {
        [Key]
        public long challenge_id { get; set; }

        public long user_id { get; set; }

        public string challenge_token_hash { get; set; } = string.Empty;

        public DateTime created_at { get; set; } = DateTime.UtcNow;

        public DateTime expires_at { get; set; }

        public DateTime? used_at { get; set; }

        public int failed_attempts { get; set; } = 0;
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Authentication.Application.DTO
{
    public class LoginResponseDto
    {
        public string access_token { get; set; } = string.Empty;
        public string refresh_token { get; set; } = string.Empty;
        public string token_type { get; set; } = "Bearer";
        public DateTime expires_at { get; set; }
        public long user_id { get; set; }
        public string username { get; set; } = string.Empty;
        public string role_name { get; set; } = string.Empty;
    }
}

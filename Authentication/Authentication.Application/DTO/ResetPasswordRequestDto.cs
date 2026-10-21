using System;
using System.Collections.Generic;
using System.Text;

namespace Authentication.Application.DTO
{
    public class ResetPasswordRequestDto
    {
        public string username { get; set; } = string.Empty;
        public string reset_token { get; set; } = string.Empty;
        public string new_password { get; set; } = string.Empty;
    }
}

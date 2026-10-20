using System;
using System.Collections.Generic;
using System.Text;

namespace Authentication.Application.DTO
{
    public class LoginRequestDto
    {
        public string username { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
    }
}

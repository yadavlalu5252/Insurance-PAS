using System;
using System.Collections.Generic;
using System.Text;

namespace Authentication.Application.DTO
{
    public class ChangePasswordRequestDto
    {
        public string current_password { get; set; } = string.Empty;
        public string new_password { get; set; } = string.Empty;
    }
}

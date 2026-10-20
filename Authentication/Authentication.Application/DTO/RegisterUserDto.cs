using System;
using System.Collections.Generic;
using System.Text;

namespace Authentication.Application.DTO
{
    public class RegisterUserDto
    {
        public long employee_id { get; set; }
        public string username { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string? email { get; set; }
        public string? mobile { get; set; }
        public long role_id { get; set; }
        public long branch_id { get; set; }
    }
}

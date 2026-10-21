using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

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
    }
}

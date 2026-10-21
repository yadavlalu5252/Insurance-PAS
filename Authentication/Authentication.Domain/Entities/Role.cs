using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Authentication.Domain.Entities
{
    public class Role
    {
        [Key]
        public long role_id { get; set; }

        public string role_name { get; set; } = string.Empty;

        public string? description { get; set; }
    }
}

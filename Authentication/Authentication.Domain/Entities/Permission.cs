using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Authentication.Domain.Entities
{
    public class Permission
    {
        [Key]
        public long permission_id { get; set; }

        public string permission_name { get; set; } = string.Empty;

        public string module_name { get; set; } = string.Empty;

        public string action_type { get; set; } = string.Empty;
    }
}

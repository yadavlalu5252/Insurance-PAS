using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Authentication.Domain.Entities
{
    public class RolePermission
    {
        [Key]
        public long role_permission_id { get; set; }

        public long role_id { get; set; }

        public long permission_id { get; set; }
    }
}

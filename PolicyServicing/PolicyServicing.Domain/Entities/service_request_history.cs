using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PolicyServicing.Domain.Entities
{
    public class service_request_history
    {
        [Key]
        public long history_id { get; set; }


        [ForeignKey("request_id")]
        public long request_id { get; set; }

        public service_request Service_Request { get; set; }

        public string action { get; set; } = string.Empty;

        public long action_by { get; set; }

        public DateTime action_date { get; set; }

        public string remarks { get; set; } = string.Empty;
    }
}

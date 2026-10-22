using System;
using System.Collections.Generic;
using System.Text;

namespace PolicyServicing.Application.DTO
{
    using System;

    namespace PolicyServicing.Application.DTO
    {
        public class CustomerDTO
        {
            public long customer_id { get; set; }

            public string customer_code { get; set; }

            public string first_name { get; set; }

            public string? middle_name { get; set; }

            public string last_name { get; set; }

            public DateOnly date_of_birth { get; set; }

            public string gender { get; set; }

            public string marital_status { get; set; }

            public string occupation { get; set; }


            public decimal annual_income { get; set; }

            public string mobile_number { get; set; }

            public string email { get; set; }

            public string customer_status { get; set; }

            public DateTime created_at { get; set; }

            public DateTime updated_at { get; set; }
        }
    }
}

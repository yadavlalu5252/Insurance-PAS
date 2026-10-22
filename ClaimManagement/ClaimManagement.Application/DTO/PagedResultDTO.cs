using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.DTO
{
    public class PagedResultDTO<T>
    {
        public List<T> items { get; set; } = new();
        public int page_number { get; set; }
        public int page_size { get; set; }
        public int total_records { get; set; }
        public int total_pages { get; set; }
    }
}

using PolicyServicing.Application.DTO;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace PolicyServicing.Infrastructure.Config
{
    // 1. Catch her ApiResponse wrapper (from her ClaimsController)
    public class ClaimsApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }

    // 2. Catch her PagedResultDTO wrapper (matches exactly what you just shared)
    public class ClaimsPagedResult<T>
    {
        public List<T> items { get; set; } = new();
        public int page_number { get; set; }
        public int page_size { get; set; }
        public int total_records { get; set; }
        public int total_pages { get; set; }
    }

    public class ClaimsClient
    {
        HttpClient client;

        public ClaimsClient(HttpClient client)
        {
            this.client = client;
        }

        private void SetAuthHeader(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
            }
        }

        public async Task<List<ClaimDTO>> GetAllClaims(string token)
        {
            SetAuthHeader(token);


            var url = "/api/v1/claims?page_number=1&page_size=1000";


            var response = await client.GetFromJsonAsync<ClaimsApiResponse<ClaimsPagedResult<ClaimDTO>>>(url);

   
            return response?.Data?.items ?? new List<ClaimDTO>();
        }
    }
}
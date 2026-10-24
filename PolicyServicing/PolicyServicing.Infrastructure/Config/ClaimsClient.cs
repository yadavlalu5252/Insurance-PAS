using PolicyServicing.Application.DTO;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace PolicyServicing.Infrastructure.Config
{
    // 1. Match her exact ApiResponse structure
    public class ClaimsApiResponse<T>
    {
        public bool success { get; set; }
        public string message { get; set; }
        public T data { get; set; }
        public List<string> errors { get; set; }
    }

    // 2. Match her exact PagedResultDTO structure (from your previous message)
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

            // 🚨 Change page_size to 10 just to test if 1000 is too high
            var url = "/api/v1/claims?page_number=1&page_size=10";

            // 1. Send the request manually to read the error body
            var response = await client.GetAsync(url);

            // 2. If it fails, read her error message and throw it!
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"Claims API Error: {response.StatusCode} - {errorBody}");
            }

            // 3. If successful, parse the JSON normally
            var responseData = await response.Content.ReadFromJsonAsync<ClaimsApiResponse<ClaimsPagedResult<ClaimDTO>>>();
            return responseData?.data?.items ?? new List<ClaimDTO>();
        }
    }
}
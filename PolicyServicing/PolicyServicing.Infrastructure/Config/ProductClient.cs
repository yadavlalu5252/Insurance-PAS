using PolicyServicing.Application.DTO;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace PolicyServicing.Infrastructure.Config
{
    // 🚨 Add this to catch his SuccessResponse wrapper
    public class ProductApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }

    public class ProductClient
    {
        HttpClient client;

        public ProductClient(HttpClient client)
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

        public async Task<List<ProductRiderDTO>> GetEligibleRiders(long productId, string token)
        {
            SetAuthHeader(token);

            // 🚨 Notice the route change to /insurance-products/ and the wrapper
            var response = await client.GetFromJsonAsync<ProductApiResponse<List<ProductRiderDTO>>>($"/api/v1/insurance-products/{productId}/riders");
            return response?.Data ?? new List<ProductRiderDTO>();
        }

        public async Task<List<CoverageMasterDTO>> GetProductCoverages(long productId, string token)
        {
            SetAuthHeader(token);

            // 🚨 Notice the route change to /insurance-products/ and the wrapper
            var response = await client.GetFromJsonAsync<ProductApiResponse<List<CoverageMasterDTO>>>($"/api/v1/insurance-products/{productId}/coverages");
            return response?.Data ?? new List<CoverageMasterDTO>();
        }

        // Add this inside ProductClient.cs
        public async Task<CoverageMasterDTO> GetCoverageRules(long coverageId, string token)
        {
            SetAuthHeader(token);
            // Hits her GET /api/v1/coverages/{id} endpoint
            var response = await client.GetFromJsonAsync<ProductApiResponse<CoverageMasterDTO>>($"/api/v1/coverages/{coverageId}");
            return response?.Data;
        }
    }
}
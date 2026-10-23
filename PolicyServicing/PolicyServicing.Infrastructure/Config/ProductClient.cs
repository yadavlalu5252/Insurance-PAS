using PolicyServicing.Application.DTO;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace PolicyServicing.Infrastructure.Config
{
    public class ProductClient
    {
        HttpClient client;

        public ProductClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task<List<ProductRiderDTO>> GetEligibleRiders(long productId)
        {
            return await client.GetFromJsonAsync<List<ProductRiderDTO>>($"/api/v1/products/{productId}/riders");
        }

        // In ProductClient.cs
        public async Task<List<CoverageMasterDTO>> GetProductCoverages(long productId)
        {
            // This calls the GET /api/v1/products/{id}/coverages endpoint
            return await client.GetFromJsonAsync<List<CoverageMasterDTO>>($"/api/v1/products/{productId}/coverages");
        }
    }
}

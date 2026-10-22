using PolicyServicing.Application.DTO;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace PolicyServicing.Infrastructure.Config
{
    public class ClaimsClient
    {
        HttpClient client;

        public ClaimsClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task<List<ClaimDTO>> GetAllClaims()
        {
            return await client.GetFromJsonAsync<List<ClaimDTO>>("/api/v1/claims");
        }

    }
}

using PremiumSchedule.Application.Common;
using PremiumSchedule.Application.DTO;
using PremiumSchedule.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace PremiumSchedule.Infrastructure.Repositories
{
    public class QuoteClient:IQuoteClient
    {
        private readonly HttpClient httpClient;

        public QuoteClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<QuoteDto?> GetQuoteByIdAsync(int quoteId)
        {
            if (quoteId <= 0)
            {
                return null;
            }

            var response = await httpClient.GetAsync($"Get/{quoteId}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var result =
                await response.Content.ReadFromJsonAsync<
                    ResponseWrapper<QuoteDto>
                >();

            return result?.Data;
        }

    }
}

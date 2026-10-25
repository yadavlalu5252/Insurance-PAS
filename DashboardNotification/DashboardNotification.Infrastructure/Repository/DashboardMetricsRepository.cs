
using DashboardNotification.Application.DTO;
using DashboardNotification.Application.Interface;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace DashboardNotification.Infrastructure.Repository
{
    public class DashboardMetricsRepository : IDashboardMetrics
    {
        private readonly HttpClient http;
        private readonly IConfiguration configuration;

        public DashboardMetricsRepository(
            HttpClient http,
            IConfiguration configuration)
        {
            this.http = http;
            this.configuration = configuration;
        }

        public async Task<DashboardMetrics> GetDashboardMetrics(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new UnauthorizedAccessException(
                    "The dashboard request did not contain an access token.");
            }

            string baseUrl =
                configuration["AuthenticationApi:BaseUrl"]?.TrimEnd('/')
                ?? throw new InvalidOperationException(
                    "AuthenticationApi:BaseUrl is missing.");

            string userResponse = await GetApiResponse(
                baseUrl + "/api/v1/users", token);

            string roleResponse = await GetApiResponse(
                baseUrl + "/api/v1/roles", token);

            using JsonDocument users = JsonDocument.Parse(userResponse);
            using JsonDocument roles = JsonDocument.Parse(roleResponse);

            DashboardMetrics data = new DashboardMetrics();

            foreach (JsonElement user in users.RootElement.EnumerateArray())
            {
                data.TotalUsers++;

                string status =
                    user.GetProperty("status").GetString() ?? "";

                if (status == "Active")
                {
                    data.ActiveUsers++;
                }

                if (status == "Locked")
                {
                    data.LockedUsers++;
                }
            }

            data.TotalRoles = roles.RootElement.GetArrayLength();

            return data;
        }

        private async Task<string> GetApiResponse(
            string url, string token)
        {
            using HttpRequestMessage request =
                new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token.Trim());

            using HttpResponseMessage response =
                await http.SendAsync(request);

            string responseBody =
                await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new HttpRequestException(
                    "Authentication API rejected the forwarded access token. " +
                    "Check that it is valid, unexpired, and belongs to an Admin.",
                    null,
                    HttpStatusCode.Unauthorized);
            }

            response.EnsureSuccessStatusCode();

            return responseBody;
        }
    }
}

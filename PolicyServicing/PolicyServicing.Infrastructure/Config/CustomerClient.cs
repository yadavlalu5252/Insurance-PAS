using Microsoft.AspNetCore.Http;
using PolicyServicing.Application.DTO;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace PolicyServicing.Infrastructure.Config
{

    public class CustomerApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }

    public class CustomerClient
    {
        HttpClient client;

        public CustomerClient(HttpClient client)
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

        public async Task<List<CustomerAddressDTO>> GetCustomerAddresses(long customerId,string token)
        {
            SetAuthHeader(token);
            var url = $"/api/customers/{customerId}/addresses"; 
            var response = await client.GetFromJsonAsync<CustomerApiResponse<List<CustomerAddressDTO>>>(url);
            return response?.Data ?? new List<CustomerAddressDTO>();
        }

        public async Task<bool> UpdateAddress(long addressId, CustomerAddressDTO addressDTO,string token)
        {
            SetAuthHeader(token);

            var response = await client.PutAsJsonAsync($"/api/customer-addresses/{addressId}", addressDTO);
            return response.IsSuccessStatusCode;
        }

        public async Task<CustomerDTO> GetCustomerById(long customerId,string token)
        {
            SetAuthHeader(token);
            var response = await client.GetFromJsonAsync<CustomerApiResponse<CustomerDTO>>($"/api/customers/{customerId}");
            return response?.Data;
        }

        public async Task<bool> UpdateCustomer(long customerId, object customerData,string token)
        {
            SetAuthHeader(token);
            var response = await client.PutAsJsonAsync($"/api/customers/{customerId}", customerData);
            return response.IsSuccessStatusCode;
        }

        // Add this to CustomerClient.cs
        public async Task<List<CustomerDTO>> SearchCustomersByEmail(string email, string token)
        {
            SetAuthHeader(token);
            // Matches her DTO property: email
            var response = await client.GetFromJsonAsync<CustomerApiResponse<List<CustomerDTO>>>($"/api/customers/search?email={email}");
            return response?.Data ?? new List<CustomerDTO>();
        }

        public async Task<List<CustomerDTO>> SearchCustomersByMobile(string mobile, string token)
        {
            SetAuthHeader(token);
            // Matches her DTO property: mobile_number
            var response = await client.GetFromJsonAsync<CustomerApiResponse<List<CustomerDTO>>>($"/api/customers/search?mobile_number={mobile}");
            return response?.Data ?? new List<CustomerDTO>();
        }

        public async Task<List<NomineeDTO>> GetNominees(long customerId,string token)
        {
            SetAuthHeader(token);
            var response = await client.GetFromJsonAsync<CustomerApiResponse<List<NomineeDTO>>>($"/api/customers/{customerId}/nominees");
            return response?.Data ?? new List<NomineeDTO>();
        }

        public async Task<bool> AddNominee(long customerId, NomineeDTO nominee,string token)
        {
            SetAuthHeader(token);

            var response = await client.PostAsJsonAsync($"/api/customers/{customerId}/nominees", nominee);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateNominee(long nomineeId, NomineeDTO nominee,string token)
        {

            SetAuthHeader(token);

            var response = await client.PutAsJsonAsync($"/api/nominees/{nomineeId}", nominee);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteNominee(long nomineeId,string token)
        {
            SetAuthHeader(token);

            var response = await client.DeleteAsync($"/api/nominees/{nomineeId}");
            return response.IsSuccessStatusCode;
        }
    }
}
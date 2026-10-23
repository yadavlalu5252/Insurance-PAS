using PolicyServicing.Application.DTO;
using PolicyServicing.Application.DTO.PolicyServicing.Application.DTO;
using System;
using System.Collections.Generic;
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

        public async Task<List<CustomerAddressDTO>> GetCustomerAddresses(long customerId)
        {
            var url = $"/api/customers/{customerId}/addresses"; 
            var response = await client.GetFromJsonAsync<CustomerApiResponse<List<CustomerAddressDTO>>>(url);
            return response?.Data ?? new List<CustomerAddressDTO>();
        }

        public async Task<bool> UpdateAddress(long addressId, CustomerAddressDTO addressDTO)
        {
            
            var response = await client.PutAsJsonAsync($"/api/customer-addresses/{addressId}", addressDTO);
            return response.IsSuccessStatusCode;
        }

        public async Task<CustomerDTO> GetCustomerById(long customerId)
        {
            var response = await client.GetFromJsonAsync<CustomerApiResponse<CustomerDTO>>($"/api/customers/{customerId}");
            return response?.Data;
        }

        public async Task<bool> UpdateCustomer(long customerId, object customerData)
        {
            var response = await client.PutAsJsonAsync($"/api/customers/{customerId}", customerData);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<CustomerDTO>> SearchCustomers(string query)
        {
            var response = await client.GetFromJsonAsync<CustomerApiResponse<List<CustomerDTO>>>($"/api/customers/search?q={query}");
            return response?.Data ?? new List<CustomerDTO>();
        }


        public async Task<List<NomineeDTO>> GetNominees(long customerId)
        {
            var response = await client.GetFromJsonAsync<CustomerApiResponse<List<NomineeDTO>>>($"/api/customers/{customerId}/nominees");
            return response?.Data ?? new List<NomineeDTO>();
        }

        public async Task<bool> AddNominee(long customerId, NomineeDTO nominee)
        {
     
            var response = await client.PostAsJsonAsync($"/api/customers/{customerId}/nominees", nominee);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateNominee(long nomineeId, NomineeDTO nominee)
        {
 
            var response = await client.PutAsJsonAsync($"/api/nominees/{nomineeId}", nominee);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteNominee(long nomineeId)
        {

            var response = await client.DeleteAsync($"/api/nominees/{nomineeId}");
            return response.IsSuccessStatusCode;
        }
    }
}
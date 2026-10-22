using PolicyServicing.Application.DTO;
using PolicyServicing.Application.DTO.PolicyServicing.Application.DTO;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace PolicyServicing.Infrastructure.Config
{
    public class CustomerClient
    {
        HttpClient client;

        public CustomerClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task<List<CustomerAddressDTO>> GetCustomerAddresses(long customerId)
        {
            return await client.GetFromJsonAsync<List<CustomerAddressDTO>>($"/api/v1/customers/{customerId}/addresses");
        }


        public async Task<bool> UpdateAddress(long addressId, CustomerAddressDTO addressDTO)
        {
            var response = await client.PutAsJsonAsync($"/api/v1/addresses/{addressId}", addressDTO);
            return response.IsSuccessStatusCode;
        }

        public async Task<CustomerDTO> GetCustomerById(long customerId)
        {
            return await client.GetFromJsonAsync<CustomerDTO>($"/api/v1/customers/{customerId}");
        }

        public async Task<bool> UpdateCustomer(long customerId, object customerData)
        {
            var response = await client.PutAsJsonAsync($"/api/v1/customers/{customerId}", customerData);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<CustomerDTO>> SearchCustomers(string query)
        {
            return await client.GetFromJsonAsync<List<CustomerDTO>>($"/api/v1/customers/search?q={query}");
        }

        public async Task<List<NomineeDTO>> GetNominees(long customerId)
        {
            return await client.GetFromJsonAsync<List<NomineeDTO>>($"/api/v1/customers/{customerId}/nominees");
        }

        public async Task<bool> AddNominee(long customerId, NomineeDTO nominee)
        {
            var response = await client.PostAsJsonAsync($"/api/v1/customers/{customerId}/nominees", nominee);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateNominee(long nomineeId, NomineeDTO nominee)
        {
            var response = await client.PutAsJsonAsync($"/api/v1/nominees/{nomineeId}", nominee);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteNominee(long nomineeId)
        {
            var response = await client.DeleteAsync($"/api/v1/nominees/{nomineeId}");
            return response.IsSuccessStatusCode;
        }
    }
    }

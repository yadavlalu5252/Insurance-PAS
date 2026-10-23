using PolicyServicing.Application.DTO;
using PolicyServicing.Application.Interface;
using PolicyServicing.Domain.Entities;
using PolicyServicing.Infrastructure.Config;
using PolicyServicing.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PolicyServicing.Infrastructure.Repository
{
    public class UpdateContactService : IUpdateContact
    {
        private readonly AppDbContext db;
        private readonly CustomerClient customerClient;

        private readonly OtpService otp;
        public UpdateContactService(AppDbContext db, CustomerClient customerClient,OtpService otp)
        {
            this.db = db;
            this.customerClient = customerClient;
            this.otp = otp;
        }

        public async Task<string> ProcessContactUpdateAsync(EndorsementRequestDTO request)
        {
            var contactData = JsonSerializer.Deserialize<ContactUpdateDTO>(request.PayloadJson);

            if (!await otp.VerifyAsync(contactData.EmailAddress, contactData.OtpCode))
                throw new Exception("Invalid or expired OTP for the new email.");


            // Read from Customer Client for Duplicate Check
            var existingEmail = await customerClient.SearchCustomers(contactData.EmailAddress);
            if (existingEmail.Any(c => c.customer_id != request.CustomerId))
                throw new Exception("Email address is already in use by another customer.");

            var existingMobile = await customerClient.SearchCustomers(contactData.MobileNumber);
            if (existingMobile.Any(c => c.customer_id != request.CustomerId))
                throw new Exception("Mobile number is already in use by another customer.");


            var oldCustomer = await customerClient.GetCustomerById(request.CustomerId);
            string previousValueJson = JsonSerializer.Serialize(oldCustomer);

            var newCustomerData = new
            {
                mobile_number = contactData.MobileNumber,
                email = contactData.EmailAddress
            };
            string updatedValueJson = JsonSerializer.Serialize(newCustomerData);


            var serviceRequest = new service_request
            {
                request_number = $"SR-{DateTime.UtcNow.Ticks}",
                policy_id = request.PolicyId,
                customer_id = request.CustomerId,
                request_type = "Contact Update",
                request_date = DateTime.UtcNow,
                status = RequestStatus.UnderReview // Set to UnderReview
            };
            db.service_request.Add(serviceRequest);
            await db.SaveChangesAsync();

 
            db.policy_endorsements.Add(new policy_endorsement
            {
                request_id = serviceRequest.request_id,
                endorsement_number = $"END-{DateTime.UtcNow.Ticks}",
                endorsement_type = "Contact Update",
                previous_value = previousValueJson,
                updated_value = updatedValueJson,
                approval_status = Approval_status.Pending // Set to Pending
            });


            db.service_request_history.Add(new service_request_history
            {
                request_id = serviceRequest.request_id,
                action = "Contact Update Requested (Pending Admin)",
                action_by = request.CustomerId,
                action_date = DateTime.UtcNow,
                remarks = $"Mobile changed to {contactData.MobileNumber}. OTP verified."
            });

            await db.SaveChangesAsync();

            return serviceRequest.request_number;
        }
    }
}
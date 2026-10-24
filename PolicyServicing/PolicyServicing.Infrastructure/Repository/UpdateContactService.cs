using PolicyServicing.Application.DTO;
using PolicyServicing.Application.Interface;
using PolicyServicing.Domain.Entities;
using PolicyServicing.Infrastructure.Data;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace PolicyServicing.Infrastructure.Repository
{
    public class UpdateContactService : IUpdateContact
    {
        private readonly AppDbContext db;
        private readonly OtpService otp;

        // 🚨 CustomerClient is completely removed!
        public UpdateContactService(AppDbContext db, OtpService otp)
        {
            this.db = db;
            this.otp = otp;
        }

        public async Task<string> ProcessContactUpdateAsync(EndorsementRequestDTO request)
        {
            var contactData = JsonSerializer.Deserialize<ContactUpdateDTO>(request.PayloadJson);


            if (!await otp.VerifyAsync(contactData.EmailAddress, contactData.OtpCode))
                throw new Exception("Invalid or expired OTP for the new email.");

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
                status = RequestStatus.UnderReview
            };
            db.service_request.Add(serviceRequest);
            await db.SaveChangesAsync();


            db.policy_endorsements.Add(new policy_endorsement
            {
                request_id = serviceRequest.request_id,
                endorsement_number = $"END-{DateTime.UtcNow.Ticks}",
                endorsement_type = "Contact Update",
                previous_value = "{}", // 🚨 Admin will fill this
                updated_value = updatedValueJson,
                approval_status = Approval_status.Pending
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
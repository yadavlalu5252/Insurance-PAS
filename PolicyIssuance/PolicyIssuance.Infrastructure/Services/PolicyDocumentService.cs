
using Microsoft.EntityFrameworkCore;
using PolicyIssuance.Application.DTOs;
using PolicyIssuance.Application.Interface;
using PolicyIssuance.Infrastructure.Data;

namespace PolicyIssuance.Infrastructure.Services
{
    public class PolicyDocumentService : IPolicyDocumentService
    {
        private readonly AppDbContext db;

        public PolicyDocumentService(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<PolicyDocumentDTO?> GetDocumentAsync(
            long policy_id)
        {
            var document = await db.PolicyDocuments
                .AsNoTracking()
                .Where(d => d.policy_id == policy_id)
                .OrderByDescending(d => d.version)
                .FirstOrDefaultAsync();

            if (document == null)
                return null;

            return new PolicyDocumentDTO
            {
                policy_document_id = document.policy_document_id,
                policy_id = document.policy_id,
                document_name = document.document_name,
                document_path = document.document_path,
                version = document.version,
                generated_date = document.generated_date
            };
        }

        public async Task<PolicyDocumentDTO?> RegenerateDocumentAsync(
            long policy_id)
        {
            await Task.CompletedTask;
            return null;
        }
    }
}

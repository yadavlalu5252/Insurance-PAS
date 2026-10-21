using PolicyIssuance.Application.DTOs;

namespace PolicyIssuance.Application.Interface
{
    public interface IPolicyDocumentService
    {
        Task<PolicyDocumentDTO?> GetDocumentAsync(long policy_id);

        Task<PolicyDocumentDTO?> RegenerateDocumentAsync(long policy_id);
    }
}
using InsuranceProductManagement.Application.DTO;

namespace InsuranceProductManagement.Application.Interface
{
    public interface IBenefitMasterService
    {
        Task<List<BenefitMasterDTO>> GetAllAsync();

        Task<BenefitMasterDTO?> GetByIdAsync(int id);

        Task<BenefitMasterDTO> CreateAsync(BenefitMasterDTO dto);

        Task<BenefitMasterDTO?> UpdateAsync(int id, BenefitMasterDTO dto);

        Task<bool> DeleteAsync(int id);
    }
}
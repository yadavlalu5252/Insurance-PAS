using InsuranceProductManagement.Application.DTO;

namespace InsuranceProductManagement.Application.Interface
{
    public interface IPremiumFrequencyService
    {
        Task<List<PremiumFrequencyDTO>> GetAllAsync();

        Task<PremiumFrequencyDTO?> GetByIdAsync(int id);

        Task<PremiumFrequencyDTO> CreateAsync(PremiumFrequencyDTO dto);

        Task<PremiumFrequencyDTO?> UpdateAsync(int id, PremiumFrequencyDTO dto);

        Task<bool> DeleteAsync(int id);
    }
}
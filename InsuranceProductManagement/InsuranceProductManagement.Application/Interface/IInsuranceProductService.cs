using InsuranceProductManagement.Application.DTO;

namespace InsuranceProductManagement.Application.Interface
{
    public interface IInsuranceProductService
    {
        Task<List<InsuranceProductDTO>> GetAllAsync();

        Task<InsuranceProductDTO?> GetByIdAsync(int id);

        Task<InsuranceProductDTO> CreateAsync(InsuranceProductDTO dto);

        Task<InsuranceProductDTO?> UpdateAsync(int id, InsuranceProductDTO dto);

        Task<bool> DeleteAsync(int id);
    }
}
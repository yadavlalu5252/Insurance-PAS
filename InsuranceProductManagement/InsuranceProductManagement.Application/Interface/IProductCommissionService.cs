using InsuranceProductManagement.Application.DTO;

namespace InsuranceProductManagement.Application.Interface
{
    public interface IProductCommissionService
    {
        Task<List<ProductCommissionDTO>> GetAllAsync();

        Task<ProductCommissionDTO?> GetByIdAsync(int id);

        Task<ProductCommissionDTO> CreateAsync(ProductCommissionDTO dto);

        Task<ProductCommissionDTO?> UpdateAsync(int id, ProductCommissionDTO dto);

        Task<bool> DeleteAsync(int id);
    }
}
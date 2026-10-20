using InsuranceProductManagement.Application.DTO;

namespace InsuranceProductManagement.Application.Interface
{
    public interface ICoverageMasterService
    {
        Task<List<CoverageMasterDTO>> GetAllAsync();

        Task<CoverageMasterDTO?> GetByIdAsync(int id);

        Task<CoverageMasterDTO> CreateAsync(CoverageMasterDTO dto);

        Task<CoverageMasterDTO?> UpdateAsync(int id, CoverageMasterDTO dto);

        Task<bool> DeleteAsync(int id);
    }
}
using InsuranceProductManagement.Application.DTO;

namespace InsuranceProductManagement.Application.Interface
{
    public interface IRiderMasterService
    {
        Task<List<RiderMasterDTO>> GetAllAsync();

        Task<RiderMasterDTO?> GetByIdAsync(int id);

        Task<RiderMasterDTO> CreateAsync(RiderMasterDTO dto);

        Task<RiderMasterDTO?> UpdateAsync(int id, RiderMasterDTO dto);

        Task<bool> DeleteAsync(int id);
    }
}
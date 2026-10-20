
using CustomerRegistration.Application.DTOs.Nominee;

namespace CustomerRegistration.Application.Interfaces;

public interface INomineeService
{
    Task<NomineeResponseDto?> AddNominee(long customerId, CreateNomineeDto dto);
    Task<List<NomineeResponseDto>> GetNominees(long customerId);
    Task<NomineeResponseDto?> UpdateNominee(long nomineeId, UpdateNomineeDto dto);
    Task<bool> DeleteNominee(long nomineeId);
}

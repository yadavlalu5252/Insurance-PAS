
using Authentication.Application.DTO;

namespace Authentication.Application.Interface;

public interface IUserService
{
    Task<UserResponseDto> CreateUser(CreateUserDto request);
    Task<List<UserResponseDto>> GetAllUsers();
    Task<UserResponseDto?> GetUserById(long id);
    Task<UserResponseDto?> UpdateUser(long id, UpdateUserDto request);
}

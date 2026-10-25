using Authentication.Application.DTOs;

namespace Authentication.Application.Interfaces;

public interface IRoleService
{
    Task<RoleResponseDto> CreateRoleAsync(CreateRoleDto dto);

    Task<List<RoleResponseDto>> GetRolesAsync();
}
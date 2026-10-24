using Authentication.Application.DTOs;
using Authentication.Application.Interfaces;
using Authentication.Domain.Entities;
using Authentication.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Infrastructure.Services;

public class RoleService : IRoleService
{
    private readonly AppDbContext _context;

    public RoleService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RoleResponseDto> CreateRoleAsync(CreateRoleDto dto)
    {
        var roleName = dto.role_name.Trim();

        bool exists = await _context.Set<Role>()
            .AnyAsync(r => r.role_name == roleName);

        if (exists)
        {
            throw new InvalidOperationException("Role already exists.");
        }

        var role = new Role
        {
            role_name = roleName,
            description = dto.description?.Trim()
        };

        _context.Set<Role>().Add(role);
        await _context.SaveChangesAsync();

        return new RoleResponseDto
        {
            role_id = role.role_id,
            role_name = role.role_name,
            description = role.description
        };
    }

    public async Task<List<RoleResponseDto>> GetRolesAsync()
    {
        return await _context.Set<Role>()
            .AsNoTracking()
            .OrderBy(r => r.role_id)
            .Select(r => new RoleResponseDto
            {
                role_id = r.role_id,
                role_name = r.role_name,
                description = r.description
            })
            .ToListAsync();
    }
}
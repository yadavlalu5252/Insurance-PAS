
using Authentication.Application.DTO;
using Authentication.Application.Interface;
using Authentication.Domain.Entities;
using Authentication.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Infrastructure.Repository;

public class UserService : IUserService
{
    private readonly AppDbContext db;

    public UserService(AppDbContext db)
    {
        this.db = db;
    }

    public async Task<UserResponseDto> CreateUser(CreateUserDto request)
    {
        if (string.IsNullOrWhiteSpace(request.username) || string.IsNullOrWhiteSpace(request.password) || request.password.Length < 8)
            throw new InvalidOperationException("Username is required and password must contain at least 8 characters.");

        if (request.employee_id <= 0 || request.branch_id <= 0 || request.role_id <= 0)
            throw new InvalidOperationException("Valid employee, branch, and role IDs are required.");

        if (await db.user_master.AnyAsync(u => u.username == request.username))
            throw new InvalidOperationException("Username already exists.");

        var role = await db.role_master.FirstOrDefaultAsync(r => r.role_id == request.role_id);

        if (role == null)
            throw new InvalidOperationException("Invalid role ID.");

        if (string.IsNullOrWhiteSpace(request.status) || (request.status != "Active" && request.status != "Inactive"))
            throw new InvalidOperationException("Status must be Active or Inactive.");

        var user = new User
        {
            employee_id = request.employee_id,
            username = request.username.Trim(),
            password_hash = BCrypt.Net.BCrypt.HashPassword(request.password),
            email = request.email,
            mobile = request.mobile,
            role_id = request.role_id,
            branch_id = request.branch_id,
            status = request.status,
            two_factor_enabled = false
        };

        db.user_master.Add(user);
        await db.SaveChangesAsync();

        return await MapUser(user);
    }

    public async Task<List<UserResponseDto>> GetAllUsers()
    {
        var users = await db.user_master
            .AsNoTracking()
            .OrderBy(u => u.user_id)
            .ToListAsync();

        var result = new List<UserResponseDto>();

        foreach (var user in users)
            result.Add(await MapUser(user));

        return result;
    }

    public async Task<UserResponseDto?> GetUserById(long id)
    {
        var user = await db.user_master
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.user_id == id);

        if (user == null)
            return null;

        return await MapUser(user);
    }

    public async Task<UserResponseDto?> UpdateUser(long id, UpdateUserDto request)
    {
        var user = await db.user_master.FirstOrDefaultAsync(u => u.user_id == id);

        if (user == null)
            return null;

        if (string.IsNullOrWhiteSpace(request.username))
            throw new InvalidOperationException("Username is required.");

        if (request.employee_id <= 0 || request.branch_id <= 0 || request.role_id <= 0)
            throw new InvalidOperationException("Valid employee, branch, and role IDs are required.");

        if (await db.user_master.AnyAsync(u => u.username == request.username && u.user_id != id))
            throw new InvalidOperationException("Username already exists.");

        var roleExists = await db.role_master.AnyAsync(r => r.role_id == request.role_id);

        if (!roleExists)
            throw new InvalidOperationException("Invalid role ID.");

        if (string.IsNullOrWhiteSpace(request.status) || (request.status != "Active" && request.status != "Inactive"))
            throw new InvalidOperationException("Status must be Active or Inactive.");

        user.employee_id = request.employee_id;
        user.username = request.username.Trim();
        user.email = request.email;
        user.mobile = request.mobile;
        user.role_id = request.role_id;
        user.branch_id = request.branch_id;
        user.status = request.status;

        await db.SaveChangesAsync();

        return await MapUser(user);
    }

    private async Task<UserResponseDto> MapUser(User user)
    {
        var roleName = await db.role_master
            .Where(r => r.role_id == user.role_id)
            .Select(r => r.role_name)
            .FirstOrDefaultAsync();

        return new UserResponseDto
        {
            user_id = user.user_id,
            employee_id = user.employee_id,
            username = user.username,
            email = user.email,
            mobile = user.mobile,
            role_id = user.role_id,
            role_name = roleName,
            branch_id = user.branch_id,
            status = user.status,
            two_factor_enabled = user.two_factor_enabled
        };
    }
}

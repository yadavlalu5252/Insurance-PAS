
using Authentication.Application.DTO;
using Authentication.Application.Interface;
using Authentication.Domain.Entities;
using Authentication.Infrastructure.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Authentication.Infrastructure.Repository
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext db;
        private readonly IConfiguration cnf;
        private readonly IMapper mapper;

        public AuthService(AppDbContext db, IConfiguration cnf, IMapper mapper)
        {
            this.db = db;
            this.cnf = cnf;
            this.mapper = mapper;
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes);
        }

        private string HashToken(string token)
        {
            var tokenBytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(tokenBytes);
        }

        private string GenerateAccessToken(User user, string roleName, out DateTime expiresAt)
        {
            var jwtKey = cnf["Jwt:Key"];
            var issuer = cnf["Jwt:Issuer"];
            var audience = cnf["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(jwtKey) || Encoding.UTF8.GetByteCount(jwtKey) < 32)
            {
                throw new InvalidOperationException("JWT Key must contain at least 32 bytes.");
            }

            if (string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
            {
                throw new InvalidOperationException("JWT Issuer or Audience is missing.");
            }

            var expiryMinutes = int.TryParse(cnf["Jwt:ExpiryMinutes"], out var minutes) && minutes > 0 ? minutes : 15;
            expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.user_id.ToString()),
                new Claim(ClaimTypes.Name, user.username),
                new Claim(ClaimTypes.Role, roleName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<LoginResponseDto> CreateLoginResponse(User user, string roleName)
        {
            var refreshToken = GenerateRefreshToken();
            var now = DateTime.UtcNow;

            db.refresh_tokens.Add(new RefreshToken
            {
                user_id = user.user_id,
                token_hash = HashToken(refreshToken),
                created_at = now,
                expires_at = now.AddDays(7)
            });

            var response = mapper.Map<LoginResponseDto>(user);
            response.access_token = GenerateAccessToken(user, roleName, out var expiresAt);
            response.refresh_token = refreshToken;
            response.token_type = "Bearer";
            response.expires_at = expiresAt;
            response.user_id = user.user_id;
            response.username = user.username;
            response.role_name = roleName;

            await db.SaveChangesAsync();

            return response;
        }

        public async Task<bool> Register(RegisterUserDto request)
        {
            if (string.IsNullOrWhiteSpace(request.username) ||
                string.IsNullOrWhiteSpace(request.password) ||
                request.password.Length < 8)
            {
                return false;
            }

            if (await db.user_master.AnyAsync(u => u.username == request.username))
            {
                return false;
            }

            if (!await db.role_master.AnyAsync(r => r.role_id == request.role_id))
            {
                return false;
            }

            var user = new User
            {
                employee_id = request.employee_id,
                username = request.username,
                password_hash = BCrypt.Net.BCrypt.HashPassword(request.password),
                email = request.email,
                mobile = request.mobile,
                role_id = request.role_id,
                branch_id = request.branch_id,
                status = "Active"
            };

            db.user_master.Add(user);
            await db.SaveChangesAsync();

            return true;
        }

        public async Task<LoginResponseDto?> Login(LoginRequestDto request)
        {
            var user = await db.user_master.FirstOrDefaultAsync(u => u.username == request.username);

            if (user == null || user.status != "Active")
            {
                return null;
            }

            if (!BCrypt.Net.BCrypt.Verify(request.password, user.password_hash))
            {
                return null;
            }

            var role = await db.role_master.FirstOrDefaultAsync(r => r.role_id == user.role_id);

            if (role == null)
            {
                return null;
            }

            return await CreateLoginResponse(user, role.role_name);
        }

        public async Task<LoginResponseDto?> RefreshToken(RefreshTokenRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.refresh_token))
            {
                return null;
            }

            var tokenHash = HashToken(request.refresh_token);
            var storedToken = await db.refresh_tokens.FirstOrDefaultAsync(t => t.token_hash == tokenHash);

            if (storedToken == null ||
                storedToken.revoked_at != null ||
                storedToken.expires_at <= DateTime.UtcNow)
            {
                return null;
            }

            var user = await db.user_master.FirstOrDefaultAsync(u => u.user_id == storedToken.user_id);

            if (user == null || user.status != "Active")
            {
                storedToken.revoked_at = DateTime.UtcNow;
                await db.SaveChangesAsync();
                return null;
            }

            var role = await db.role_master.FirstOrDefaultAsync(r => r.role_id == user.role_id);

            if (role == null)
            {
                return null;
            }

            // Revoke the old token before issuing a replacement.
            storedToken.revoked_at = DateTime.UtcNow;

            var response = await CreateLoginResponse(user, role.role_name);

            return response;
        }

        public async Task<bool> Logout(long user_id)
        {
            var activeTokens = await db.refresh_tokens
                .Where(t => t.user_id == user_id && t.revoked_at == null)
                .ToListAsync();

            if (activeTokens.Count == 0)
            {
                return true;
            }

            var now = DateTime.UtcNow;

            foreach (var token in activeTokens)
            {
                token.revoked_at = now;
            }

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ChangePassword(long user_id, string current_password, string new_password)
        {
            if (string.IsNullOrWhiteSpace(new_password) || new_password.Length < 8)
            {
                return false;
            }

            var user = await db.user_master.FirstOrDefaultAsync(u => u.user_id == user_id);

            if (user == null || user.status != "Active")
            {
                return false;
            }

            if (!BCrypt.Net.BCrypt.Verify(current_password, user.password_hash))
            {
                return false;
            }

            user.password_hash = BCrypt.Net.BCrypt.HashPassword(new_password);

            var activeTokens = await db.refresh_tokens
                .Where(t => t.user_id == user_id && t.revoked_at == null)
                .ToListAsync();

            foreach (var token in activeTokens)
            {
                token.revoked_at = DateTime.UtcNow;
            }

            await db.SaveChangesAsync();

            return true;
        }

        

        public Task<bool> ForgotPassword(string username)
        {
            throw new NotImplementedException(
                "Forgot password requires secure reset-token delivery.");
        }

        public Task<bool> ResetPassword(ResetPasswordRequestDto request)
        {
            throw new NotImplementedException("Reset password requires reset-token validation and expiry handling.");
        }
    }
}

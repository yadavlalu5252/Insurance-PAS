
using Authentication.Application.DTO;
using Authentication.Application.Interface;
using Authentication.Domain.Entities;
using Authentication.Infrastructure.Data;
using AutoMapper;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OtpNet;
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
        private readonly IDataProtector protector;

        public AuthService(
            AppDbContext db,
            IConfiguration cnf,
            IMapper mapper,
            IDataProtectionProvider dataProtectionProvider)
        {
            this.db = db;
            this.cnf = cnf;
            this.mapper = mapper;
            protector = dataProtectionProvider.CreateProtector(
                "InsurancePAS.Authentication.TOTP.v1");
        }

        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        private string GenerateRandomToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }

        private string HashToken(string token)
        {
            return Convert.ToBase64String(
                SHA256.HashData(Encoding.UTF8.GetBytes(token)));
        }

        private string GenerateAccessToken(
            User user,
            string roleName,
            out DateTime expiresAt)
        {
            var jwtKey = cnf["Jwt:Key"];
            var issuer = cnf["Jwt:Issuer"];
            var audience = cnf["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(jwtKey) ||
                Encoding.UTF8.GetByteCount(jwtKey) < 32)
            {
                throw new InvalidOperationException(
                    "JWT Key must contain at least 32 bytes.");
            }

            if (string.IsNullOrWhiteSpace(issuer) ||
                string.IsNullOrWhiteSpace(audience))
            {
                throw new InvalidOperationException(
                    "JWT Issuer or Audience is missing.");
            }

            var expiryMinutes =
                int.TryParse(cnf["Jwt:ExpiryMinutes"], out var minutes) &&
                minutes > 0 ? minutes : 15;

            expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.user_id.ToString()),
                new Claim(ClaimTypes.Name, user.username),
                new Claim(ClaimTypes.Role, roleName)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey));

            var credentials = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<LoginResponseDto> CreateLoginResponse(
            User user,
            string roleName)
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
            response.access_token =
                GenerateAccessToken(user, roleName, out var expiresAt);
            response.refresh_token = refreshToken;
            response.token_type = "Bearer";
            response.expires_at = expiresAt;
            response.user_id = user.user_id;
            response.username = user.username;
            response.role_name = roleName;

            await db.SaveChangesAsync();

            return response;
        }

        private async Task<string?> GetRoleName(long roleId)
        {
            return await db.role_master
                .Where(r => r.role_id == roleId)
                .Select(r => r.role_name)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> Register(RegisterUserDto request)
        {
            if (string.IsNullOrWhiteSpace(request.username) ||
                string.IsNullOrWhiteSpace(request.password) ||
                request.password.Length < 8)
            {
                return false;
            }

            if (await db.user_master.AnyAsync(
                u => u.username == request.username))
            {
                return false;
            }

            // Public registration always assigns Customer.
            var customerRole = await db.role_master
                .FirstOrDefaultAsync(r => r.role_name == "Customer");

            if (customerRole == null)
            {
                throw new InvalidOperationException(
                    "Customer role is missing. Seed roles before registration.");
            }

            var user = new User
            {
                employee_id = request.employee_id,
                username = request.username,
                password_hash =
                    BCrypt.Net.BCrypt.HashPassword(request.password),
                email = request.email,
                mobile = request.mobile,
                role_id = customerRole.role_id,
                branch_id = request.branch_id,
                status = "Active",
                two_factor_enabled = false
            };

            db.user_master.Add(user);
            await db.SaveChangesAsync();

            return true;
        }

        public async Task<LoginResultDto?> Login(LoginRequestDto request)
        {
            var user = await db.user_master
                .FirstOrDefaultAsync(u => u.username == request.username);

            if (user == null || user.status != "Active" ||
                !BCrypt.Net.BCrypt.Verify(
                    request.password, user.password_hash))
            {
                return null;
            }

            if (user.two_factor_enabled)
            {
                if (string.IsNullOrWhiteSpace(user.two_factor_secret))
                    return null;

                var challenge = GenerateRandomToken();
                var now = DateTime.UtcNow;

                db.two_factor_login_challenges.Add(
                    new TwoFactorLoginChallenge
                    {
                        user_id = user.user_id,
                        challenge_token_hash = HashToken(challenge),
                        created_at = now,
                        expires_at = now.AddMinutes(5)
                    });

                await db.SaveChangesAsync();

                return new LoginResultDto
                {
                    requires_two_factor = true,
                    challenge_token = challenge,
                    challenge_expires_at = now.AddMinutes(5),
                    login = null
                };
            }

            var roleName = await GetRoleName(user.role_id);

            if (roleName == null)
                return null;

            var loginResponse = await CreateLoginResponse(user, roleName);

            return new LoginResultDto
            {
                requires_two_factor = false,
                login = loginResponse
            };
        }

        public async Task<TwoFactorSetupResponseDto> SetupTwoFactor(long user_id)
        {
            var user = await db.user_master
                .FirstOrDefaultAsync(u => u.user_id == user_id);

            if (user == null || user.status != "Active")
                throw new InvalidOperationException("User not found or inactive.");

            if (user.two_factor_enabled)
                throw new InvalidOperationException(
                    "Two-factor authentication is already enabled.");

            var secretBytes = KeyGeneration.GenerateRandomKey(20);
            var secret = Base32Encoding.ToString(secretBytes);

            // Store an encrypted pending secret until verification succeeds.
            user.two_factor_secret = protector.Protect(secret);
            await db.SaveChangesAsync();

            var issuer = "InsurancePAS";
            var account = Uri.EscapeDataString(user.email ?? user.username);
            var issuerEncoded = Uri.EscapeDataString(issuer);

            var otpauthUrl =
                $"otpauth://totp/{issuerEncoded}:{account}" +
                $"?secret={secret}&issuer={issuerEncoded}" +
                "&digits=6&period=30";

            return new TwoFactorSetupResponseDto
            {
                shared_key = secret,
                otpauth_url = otpauthUrl
            };
        }

        public async Task<bool> EnableTwoFactor(
            long user_id,
            TwoFactorSetupVerifyDto request)
        {
            var user = await db.user_master
                .FirstOrDefaultAsync(u => u.user_id == user_id);

            if (user == null || user.status != "Active" ||
                user.two_factor_enabled ||
                string.IsNullOrWhiteSpace(user.two_factor_secret))
            {
                return false;
            }

            string secret;

            try
            {
                secret = protector.Unprotect(user.two_factor_secret);
            }
            catch (CryptographicException)
            {
                return false;
            }

            var totp = new Totp(Base32Encoding.ToBytes(secret));

            if (!totp.VerifyTotp(
                request.code,
                out long matchedStep,
                new VerificationWindow(previous: 1, future: 1)))
            {
                return false;
            }

            user.two_factor_enabled = true;
            user.two_factor_last_used_step = matchedStep;

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DisableTwoFactor(
            long user_id,
            string current_password,
            string code)
        {
            var user = await db.user_master
                .FirstOrDefaultAsync(u => u.user_id == user_id);

            if (user == null || user.status != "Active" ||
                !user.two_factor_enabled ||
                string.IsNullOrWhiteSpace(user.two_factor_secret) ||
                !BCrypt.Net.BCrypt.Verify(
                    current_password, user.password_hash))
            {
                return false;
            }

            string secret;

            try
            {
                secret = protector.Unprotect(user.two_factor_secret);
            }
            catch (CryptographicException)
            {
                return false;
            }

            var totp = new Totp(Base32Encoding.ToBytes(secret));

            if (!totp.VerifyTotp(
                code,
                out long matchedStep,
                new VerificationWindow(previous: 1, future: 1)))
            {
                return false;
            }

            if (user.two_factor_last_used_step.HasValue &&
                matchedStep <= user.two_factor_last_used_step.Value)
            {
                return false;
            }

            user.two_factor_enabled = false;
            user.two_factor_secret = null;
            user.two_factor_last_used_step = null;

            // Invalidate existing refresh tokens after disabling 2FA.
            var activeTokens = await db.refresh_tokens
                .Where(t => t.user_id == user_id && t.revoked_at == null)
                .ToListAsync();

            foreach (var token in activeTokens)
                token.revoked_at = DateTime.UtcNow;

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<LoginResponseDto?> VerifyTwoFactorLogin(
            TwoFactorVerifyDto request)
        {
            if (string.IsNullOrWhiteSpace(request.challenge_token) ||
                string.IsNullOrWhiteSpace(request.code))
            {
                return null;
            }

            var tokenHash = HashToken(request.challenge_token);
            var now = DateTime.UtcNow;

            var challenge = await db.two_factor_login_challenges
                .FirstOrDefaultAsync(c =>
                    c.challenge_token_hash == tokenHash);

            if (challenge == null ||
                challenge.used_at != null ||
                challenge.expires_at <= now ||
                challenge.failed_attempts >= 5)
            {
                return null;
            }

            var user = await db.user_master
                .FirstOrDefaultAsync(u => u.user_id == challenge.user_id);

            if (user == null || user.status != "Active" ||
                !user.two_factor_enabled ||
                string.IsNullOrWhiteSpace(user.two_factor_secret))
            {
                challenge.used_at = now;
                await db.SaveChangesAsync();
                return null;
            }

            string secret;

            try
            {
                secret = protector.Unprotect(user.two_factor_secret);
            }
            catch (CryptographicException)
            {
                challenge.used_at = now;
                await db.SaveChangesAsync();
                return null;
            }

            var totp = new Totp(Base32Encoding.ToBytes(secret));

            if (!totp.VerifyTotp(
                request.code,
                out long matchedStep,
                new VerificationWindow(previous: 1, future: 1)))
            {
                challenge.failed_attempts++;

                if (challenge.failed_attempts >= 5)
                    challenge.used_at = now;

                await db.SaveChangesAsync();
                return null;
            }

            if (user.two_factor_last_used_step.HasValue &&
                matchedStep <= user.two_factor_last_used_step.Value)
            {
                challenge.used_at = now;
                await db.SaveChangesAsync();
                return null;
            }

            var roleName = await GetRoleName(user.role_id);

            if (roleName == null)
                return null;

            // Consume the challenge and record the accepted TOTP time-step.
            challenge.used_at = now;
            user.two_factor_last_used_step = matchedStep;

            await db.SaveChangesAsync();

            return await CreateLoginResponse(user, roleName);
        }

        public async Task<LoginResponseDto?> RefreshToken(
            RefreshTokenRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.refresh_token))
                return null;

            var tokenHash = HashToken(request.refresh_token);

            var storedToken = await db.refresh_tokens
                .FirstOrDefaultAsync(t => t.token_hash == tokenHash);

            if (storedToken == null ||
                storedToken.revoked_at != null ||
                storedToken.expires_at <= DateTime.UtcNow)
            {
                return null;
            }

            var user = await db.user_master
                .FirstOrDefaultAsync(u => u.user_id == storedToken.user_id);

            if (user == null || user.status != "Active")
            {
                storedToken.revoked_at = DateTime.UtcNow;
                await db.SaveChangesAsync();
                return null;
            }

            var roleName = await GetRoleName(user.role_id);

            if (roleName == null)
                return null;

            // Revoke old token before issuing a replacement.
            storedToken.revoked_at = DateTime.UtcNow;

            return await CreateLoginResponse(user, roleName);
        }

        public async Task<bool> Logout(long user_id)
        {
            var activeTokens = await db.refresh_tokens
                .Where(t => t.user_id == user_id && t.revoked_at == null)
                .ToListAsync();

            var now = DateTime.UtcNow;

            foreach (var token in activeTokens)
                token.revoked_at = now;

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ChangePassword(
            long user_id,
            string current_password,
            string new_password)
        {
            if (string.IsNullOrWhiteSpace(new_password) ||
                new_password.Length < 8)
            {
                return false;
            }

            var user = await db.user_master
                .FirstOrDefaultAsync(u => u.user_id == user_id);

            if (user == null || user.status != "Active" ||
                !BCrypt.Net.BCrypt.Verify(
                    current_password, user.password_hash))
            {
                return false;
            }

            user.password_hash =
                BCrypt.Net.BCrypt.HashPassword(new_password);

            var activeTokens = await db.refresh_tokens
                .Where(t => t.user_id == user_id && t.revoked_at == null)
                .ToListAsync();

            var now = DateTime.UtcNow;

            foreach (var token in activeTokens)
                token.revoked_at = now;

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
            throw new NotImplementedException(
                "Reset password requires reset-token validation and expiry handling.");
        }
    }
}

using System.Collections.Concurrent;
using EV_BatteryChangeStation_Common.DTOs.AuthencationDTO;
using EV_BatteryChangeStation_Common.DTOs.RegisterDTO;
using EV_BatteryChangeStation_Repository.DBContext;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Service.Base;
using EV_BatteryChangeStation_Service.ExternalService.IService;
using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EV_BatteryChangeStation_Service.InternalService.Service;

public sealed class AuthenService : IAuthenService
{
    private sealed record PendingRegistration(string Email, string Password, string OtpCode, DateTime ExpiredAt);
    private sealed record PendingReset(string Email, string OtpCode, DateTime ExpiredAt);

    private static readonly ConcurrentDictionary<string, PendingRegistration> PendingRegistrations = new(StringComparer.OrdinalIgnoreCase);
    private static readonly ConcurrentDictionary<string, PendingReset> PendingResets = new(StringComparer.OrdinalIgnoreCase);
    private static readonly ConcurrentDictionary<string, DateTime> RevokedTokens = new(StringComparer.Ordinal);

    private readonly AppDbContext _context;
    private readonly IJWTService _jwtService;
    private readonly IPasswordHasher<Account> _passwordHasher;
    private readonly ILogger<AuthenService> _logger;

    public AuthenService(
        AppDbContext context,
        IJWTService jwtService,
        IPasswordHasher<Account> passwordHasher,
        ILogger<AuthenService> logger)
    {
        _context = context;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<IServiceResult> AuthenticationLogin(LoginDTO login)
    {
        if (string.IsNullOrWhiteSpace(login.Keyword) || string.IsNullOrWhiteSpace(login.Password))
        {
            return ServiceResponse.BadRequest("Username/email and password are required.");
        }

        var keyword = login.Keyword.Trim();
        var account = await _context.Accounts
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Email == keyword || x.Username == keyword);

        if (account is null)
        {
            return ServiceResponse.NotFound("Account not found.");
        }

        if (!string.Equals(account.Status, "ACTIVE", StringComparison.OrdinalIgnoreCase))
        {
            return ServiceResponse.Forbidden("Account is inactive.");
        }

        var verify = _passwordHasher.VerifyHashedPassword(account, account.PasswordHash, login.Password);
        if (verify == PasswordVerificationResult.Failed)
        {
            return ServiceResponse.BadRequest("Invalid credentials.");
        }

        account.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var token = _jwtService.GenerateToken(new LoginRespondDTO
        {
            AccountId = account.AccountId,
            AccountName = account.Username,
            Email = account.Email,
            RoleName = account.Role?.RoleName
        });

        return ServiceResponse.Ok("Login successful.", token);
    }

    public async Task<bool> RegisterAsync(RegisterDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
        {
            return false;
        }

        var email = dto.Email.Trim();
        var now = DateTime.UtcNow;
        var exists = await _context.Accounts.AnyAsync(x => x.Email == email);
        if (exists)
        {
            return false;
        }

        if (PendingRegistrations.TryGetValue(email, out var existingPending) &&
            existingPending.ExpiredAt >= now)
        {
            PendingRegistrations[email] = existingPending with
            {
                Password = dto.Password
            };
            LogOtp("REGISTER_REUSE", email, existingPending.OtpCode, existingPending.ExpiredAt);
            return true;
        }

        var otp = GenerateOtp();
        var expiredAt = now.AddMinutes(10);
        PendingRegistrations[email] = new PendingRegistration(
            email,
            dto.Password,
            otp,
            expiredAt);
        LogOtp("REGISTER", email, otp, expiredAt);

        return true;
    }

    public Task<string> SendOtpAsync(string email)
    {
        var normalizedEmail = email.Trim();
        var now = DateTime.UtcNow;
        if (PendingRegistrations.TryGetValue(normalizedEmail, out var existingPending) &&
            existingPending.ExpiredAt >= now)
        {
            LogOtp("SEND_OTP_REUSE", normalizedEmail, existingPending.OtpCode, existingPending.ExpiredAt);
            return Task.FromResult(existingPending.OtpCode);
        }

        var otp = GenerateOtp();
        var expiredAt = now.AddMinutes(10);
        PendingRegistrations[normalizedEmail] = new PendingRegistration(normalizedEmail, string.Empty, otp, expiredAt);
        LogOtp("SEND_OTP", normalizedEmail, otp, expiredAt);
        return Task.FromResult(otp);
    }

    public async Task<bool> VerifyOtpAsync(VerifyOtpDTO dto)
    {
        if (!PendingRegistrations.TryGetValue(dto.Email.Trim(), out var pending))
        {
            return false;
        }

        if (pending.ExpiredAt < DateTime.UtcNow || !string.Equals(pending.OtpCode, dto.OtpCode?.Trim(), StringComparison.Ordinal))
        {
            return false;
        }

        var driverRole = await _context.Roles.FirstOrDefaultAsync(x =>
            x.RoleName == "DRIVER" || x.RoleName == "CUSTOMER");
        if (driverRole is null)
        {
            _logger.LogError(
                "Unable to create account for {Email} because neither DRIVER nor CUSTOMER role exists.",
                pending.Email);
            return false;
        }

        var username = await GenerateUniqueUsernameAsync(pending.Email);
        var account = new Account
        {
            AccountId = Guid.NewGuid(),
            Username = username,
            Email = pending.Email,
            RoleId = driverRole.RoleId,
            Status = "ACTIVE",
            CreateDate = DateTime.UtcNow
        };
        account.PasswordHash = _passwordHasher.HashPassword(account, pending.Password);

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();
        PendingRegistrations.TryRemove(dto.Email.Trim(), out _);
        return true;
    }

    public Task<IServiceResult> LogoutAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return Task.FromResult<IServiceResult>(ServiceResponse.BadRequest("Token is required."));
        }

        RevokedTokens[token] = DateTime.UtcNow;
        return Task.FromResult<IServiceResult>(ServiceResponse.Ok("Logout successful."));
    }

    public bool IsTokenRevoked(string token)
    {
        return !string.IsNullOrWhiteSpace(token) && RevokedTokens.ContainsKey(token);
    }

    public async Task<IServiceResult> ForgotPasswordSendOtpAsync(ForgotPasswordRequestDTO dto)
    {
        var email = dto.Email?.Trim();
        if (string.IsNullOrWhiteSpace(email))
        {
            return ServiceResponse.BadRequest("Email is required.");
        }

        var exists = await _context.Accounts.AnyAsync(x => x.Email == email);
        if (!exists)
        {
            return ServiceResponse.NotFound("Account not found.");
        }

        var now = DateTime.UtcNow;
        if (PendingResets.TryGetValue(email, out var existingPending) &&
            existingPending.ExpiredAt >= now)
        {
            LogOtp("FORGOT_PASSWORD_REUSE", email, existingPending.OtpCode, existingPending.ExpiredAt);
            return ServiceResponse.Ok("OTP generated successfully.");
        }

        var otp = GenerateOtp();
        var expiredAt = now.AddMinutes(10);
        PendingResets[email] = new PendingReset(email, otp, expiredAt);
        LogOtp("FORGOT_PASSWORD", email, otp, expiredAt);
        return ServiceResponse.Ok("OTP generated successfully.");
    }

    public Task<IServiceResult> VerifyForgotPasswordOtpAsync(VerifyForgotOtpDTO dto)
    {
        var email = dto.Email?.Trim();
        if (string.IsNullOrWhiteSpace(email) || !PendingResets.TryGetValue(email, out var pending))
        {
            return Task.FromResult<IServiceResult>(ServiceResponse.NotFound("OTP request not found."));
        }

        if (pending.ExpiredAt < DateTime.UtcNow || !string.Equals(pending.OtpCode, dto.OtpCode?.Trim(), StringComparison.Ordinal))
        {
            return Task.FromResult<IServiceResult>(ServiceResponse.BadRequest("Invalid OTP."));
        }

        return Task.FromResult<IServiceResult>(ServiceResponse.Ok("OTP verified successfully."));
    }

    public async Task<IServiceResult> ResetPasswordAsync(ResetPasswordDTO dto)
    {
        var email = dto.Email?.Trim();
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(dto.NewPassword))
        {
            return ServiceResponse.BadRequest("Email and new password are required.");
        }

        if (!PendingResets.ContainsKey(email))
        {
            return ServiceResponse.BadRequest("OTP verification is required before resetting password.");
        }

        var account = await _context.Accounts.FirstOrDefaultAsync(x => x.Email == email);
        if (account is null)
        {
            return ServiceResponse.NotFound("Account not found.");
        }

        account.PasswordHash = _passwordHasher.HashPassword(account, dto.NewPassword);
        await _context.SaveChangesAsync();
        PendingResets.TryRemove(email, out _);
        return ServiceResponse.Ok("Password reset successfully.");
    }

    private static string GenerateOtp()
        => Random.Shared.Next(100000, 999999).ToString();

    private void LogOtp(string purpose, string email, string otp, DateTime expiredAt)
    {
        _logger.LogWarning(
            "LOCAL OTP [{Purpose}] Email={Email} OTP={Otp} ExpiresAtUtc={ExpiresAtUtc}",
            purpose,
            email,
            otp,
            expiredAt);
    }

    private async Task<string> GenerateUniqueUsernameAsync(string email)
    {
        var baseName = email.Split('@')[0].Trim();
        var candidate = baseName;
        var suffix = 1;

        while (await _context.Accounts.AnyAsync(x => x.Username == candidate))
        {
            candidate = $"{baseName}{suffix++}";
        }

        return candidate;
    }
}

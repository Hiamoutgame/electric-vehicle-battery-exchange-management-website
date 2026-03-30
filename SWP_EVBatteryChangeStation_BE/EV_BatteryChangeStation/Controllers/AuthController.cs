using EV_BatteryChangeStation_Common.DTOs.AuthencationDTO;
using EV_BatteryChangeStation_Common.DTOs.RegisterDTO;
using EV_BatteryChangeStation.Contracts.Auth;
using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EV_BatteryChangeStation.Controllers;

[Route("api/v1/auth")]
public sealed class AuthController : ApiControllerBase
{
    private readonly IAuthenService _authenService;
    private readonly IAccountService _accountService;

    public AuthController(IAuthenService authenService, IAccountService accountService)
    {
        _authenService = authenService;
        _accountService = accountService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authenService.AuthenticationLogin(new LoginDTO
        {
            Keyword = string.IsNullOrWhiteSpace(request.Email) ? request.Keyword : request.Email,
            Password = request.Password
        });

        object? data = result.Data;
        if (result.Status is >= 200 and < 300 && result.Data is string token)
        {
            data = new
            {
                accessToken = token
            };
        }

        return ApiResult(result, "LOGIN_SUCCESS", "LOGIN_FAILED", data);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
    {
        var registered = await _authenService.RegisterAsync(dto);
        if (registered)
        {
            return Ok(new
            {
                success = true,
                code = "ACCOUNT_CREATED",
                message = "OTP sent to your email.",
                data = new
                {
                    email = dto.Email
                }
            });
        }

        return BadRequest(new
        {
            success = false,
            code = "ACCOUNT_ALREADY_EXISTS",
            message = "Email already exists."
        });
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDTO dto)
    {
        var verified = await _authenService.VerifyOtpAsync(dto);
        if (verified)
        {
            return Ok(new
            {
                success = true,
                code = "OTP_VERIFIED",
                message = "Registration successful."
            });
        }

        return BadRequest(new
        {
            success = false,
            code = "OTP_INVALID",
            message = "Invalid OTP."
        });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        if (CurrentAccountId is not Guid accountId)
        {
            return MissingCurrentAccount();
        }

        var result = await _accountService.GetAccountProfileAsync(accountId);
        return ApiResult(result, "PROFILE_FETCHED", "PROFILE_NOT_FOUND");
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var authHeader = HttpContext.Request.Headers.Authorization.ToString();
        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new
            {
                success = false,
                code = "INVALID_TOKEN",
                message = "Authorization header is missing or invalid."
            });
        }

        var token = authHeader.Replace("Bearer ", string.Empty, StringComparison.OrdinalIgnoreCase);
        var result = await _authenService.LogoutAsync(token);
        return ApiResult(result, "LOGOUT_SUCCESS", "LOGOUT_FAILED");
    }
}

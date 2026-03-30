using EV_BatteryChangeStation.Contracts.Common;
using EV_BatteryChangeStation_Service.Base;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EV_BatteryChangeStation.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected Guid? CurrentAccountId
    {
        get
        {
            var rawValue = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            return Guid.TryParse(rawValue, out var accountId) ? accountId : null;
        }
    }

    protected string? CurrentRole =>
        User.FindFirstValue(ClaimTypes.Role) ??
        User.FindFirstValue("role");

    protected IActionResult ApiResult(IServiceResult result, string successCode, string? defaultErrorCode = null, object? dataOverride = null)
    {
        var response = new ApiResponse
        {
            Success = result.Status is >= 200 and < 300,
            Code = result.Status is >= 200 and < 300
                ? successCode
                : ResolveErrorCode(result, defaultErrorCode),
            Message = result.Message ?? string.Empty,
            Data = dataOverride ?? result.Data,
            Errors = result.Errors?.Select(message => new ApiFieldError { Message = message }).ToArray()
        };

        return StatusCode(result.Status, response);
    }

    protected IActionResult MissingCurrentAccount()
    {
        return StatusCode(StatusCodes.Status401Unauthorized, new ApiResponse
        {
            Success = false,
            Code = "INVALID_TOKEN",
            Message = "Unable to resolve current account from JWT token."
        });
    }

    protected IActionResult Forbidden(string message, string code = "FORBIDDEN")
    {
        return StatusCode(StatusCodes.Status403Forbidden, new ApiResponse
        {
            Success = false,
            Code = code,
            Message = message
        });
    }

    private static string ResolveErrorCode(IServiceResult result, string? defaultErrorCode)
    {
        var enumName = result.ErrorCode?.ToString();
        if (!string.IsNullOrWhiteSpace(enumName) &&
            !string.Equals(enumName, "None", StringComparison.OrdinalIgnoreCase))
        {
            return ToSnakeUpper(enumName);
        }

        return defaultErrorCode ?? $"HTTP_{result.Status}";
    }

    private static string ToSnakeUpper(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var builder = new StringBuilder();
        for (var i = 0; i < value.Length; i++)
        {
            var current = value[i];
            if (i > 0 && char.IsUpper(current) && (char.IsLower(value[i - 1]) || char.IsDigit(value[i - 1])))
            {
                builder.Append('_');
            }

            builder.Append(char.ToUpperInvariant(current));
        }

        return builder.ToString();
    }
}

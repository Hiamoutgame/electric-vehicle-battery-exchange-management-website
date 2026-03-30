using EV_BatteryChangeStation_Common.DTOs.StationDTO;
using EV_BatteryChangeStation_Common.DTOs.SubscriptionDTO;
using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EV_BatteryChangeStation.Controllers;

[Authorize]
[Route("api/v1/admin")]
public sealed class AdminController : ApiControllerBase
{
    private readonly IAccountService _accountService;
    private readonly IRevenueService _revenueService;
    private readonly IStationService _stationService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly ISupportRequestService _supportRequestService;

    public AdminController(
        IAccountService accountService,
        IRevenueService revenueService,
        IStationService stationService,
        ISubscriptionService subscriptionService,
        ISupportRequestService supportRequestService)
    {
        _accountService = accountService;
        _revenueService = revenueService;
        _stationService = stationService;
        _subscriptionService = subscriptionService;
        _supportRequestService = supportRequestService;
    }

    [HttpGet("stations")]
    public async Task<IActionResult> GetStations()
    {
        var forbidden = EnsureAdmin();
        if (forbidden is not null)
        {
            return forbidden;
        }

        var result = await _stationService.GetAllAsync();
        return ApiResult(result, "ADMIN_STATION_LIST_FETCHED", "ADMIN_STATION_LIST_FAILED");
    }

    [HttpPost("stations")]
    public async Task<IActionResult> CreateStation([FromBody] StationCreateDTO dto)
    {
        var forbidden = EnsureAdmin();
        if (forbidden is not null)
        {
            return forbidden;
        }

        var result = await _stationService.CreateAsync(dto);
        return ApiResult(result, "STATION_CREATED", "STATION_CREATE_FAILED");
    }

    [HttpPatch("stations/{stationId:guid}")]
    public async Task<IActionResult> UpdateStation(Guid stationId, [FromBody] StationCreateDTO dto)
    {
        var forbidden = EnsureAdmin();
        if (forbidden is not null)
        {
            return forbidden;
        }

        var result = await _stationService.UpdateAsync(stationId, dto);
        return ApiResult(result, "STATION_UPDATED", "STATION_UPDATE_FAILED");
    }

    [HttpDelete("stations/{stationId:guid}")]
    public async Task<IActionResult> DeleteStation(Guid stationId)
    {
        var forbidden = EnsureAdmin();
        if (forbidden is not null)
        {
            return forbidden;
        }

        var result = await _stationService.DeleteAsync(stationId);
        return ApiResult(result, "STATION_DEACTIVATED", "STATION_DELETE_FAILED");
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var forbidden = EnsureAdmin();
        if (forbidden is not null)
        {
            return forbidden;
        }

        var result = await _accountService.GetAllAccountsAsync();
        return ApiResult(result, "USER_LIST_FETCHED", "USER_LIST_FAILED");
    }

    [HttpPost("subscription-plans")]
    public async Task<IActionResult> CreateSubscriptionPlan([FromBody] SubscriptionCreateUpdateDTO dto)
    {
        var forbidden = EnsureAdmin();
        if (forbidden is not null)
        {
            return forbidden;
        }

        var result = await _subscriptionService.CreateAsync(dto);
        return ApiResult(result, "SUBSCRIPTION_PLAN_CREATED", "SUBSCRIPTION_PLAN_CREATE_FAILED");
    }

    [HttpPatch("subscription-plans/{planId:guid}")]
    public async Task<IActionResult> UpdateSubscriptionPlan(Guid planId, [FromBody] SubscriptionCreateUpdateDTO dto)
    {
        var forbidden = EnsureAdmin();
        if (forbidden is not null)
        {
            return forbidden;
        }

        var result = await _subscriptionService.UpdateAsync(planId, dto);
        return ApiResult(result, "SUBSCRIPTION_PLAN_UPDATED", "SUBSCRIPTION_PLAN_UPDATE_FAILED");
    }

    [HttpGet("support-requests")]
    public async Task<IActionResult> GetSupportRequests()
    {
        var forbidden = EnsureAdmin();
        if (forbidden is not null)
        {
            return forbidden;
        }

        var result = await _supportRequestService.GetAllAsync();
        return ApiResult(result, "SUPPORT_REQUEST_ADMIN_LIST_FETCHED", "SUPPORT_REQUEST_ADMIN_LIST_FAILED");
    }

    [HttpGet("reports/revenue")]
    public async Task<IActionResult> GetRevenueReport()
    {
        var forbidden = EnsureAdmin();
        if (forbidden is not null)
        {
            return forbidden;
        }

        var data = await _revenueService.GetRevenueByStationAsync();
        return Ok(new
        {
            success = true,
            code = "REVENUE_REPORT_FETCHED",
            message = "Revenue report fetched successfully",
            data
        });
    }

    private IActionResult? EnsureAdmin()
    {
        return string.Equals(CurrentRole, "Admin", StringComparison.OrdinalIgnoreCase)
            ? null
            : Forbidden("Only admin accounts can access this endpoint.", "ADMIN_ACCESS_REQUIRED");
    }
}

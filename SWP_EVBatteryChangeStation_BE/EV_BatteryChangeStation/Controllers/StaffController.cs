using EV_BatteryChangeStation_Common.DTOs.BatteryDTO;
using EV_BatteryChangeStation_Common.DTOs.PaymentDTO;
using EV_BatteryChangeStation_Common.DTOs.SupportRequestDTO;
using EV_BatteryChangeStation_Common.DTOs.SwappingtransactionDto;
using EV_BatteryChangeStation.Contracts.Bookings;
using EV_BatteryChangeStation.Contracts.Payments;
using EV_BatteryChangeStation.Contracts.Support;
using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EV_BatteryChangeStation.Controllers;

[Authorize]
[Route("api/v1/staff")]
public sealed class StaffController : ApiControllerBase
{
    private readonly IAccountService _accountService;
    private readonly IBatteryService _batteryService;
    private readonly IBookingService _bookingService;
    private readonly IPaymentService _paymentService;
    private readonly ISupportRequestService _supportRequestService;
    private readonly ISwappingService _swappingService;

    public StaffController(
        IAccountService accountService,
        IBatteryService batteryService,
        IBookingService bookingService,
        IPaymentService paymentService,
        ISupportRequestService supportRequestService,
        ISwappingService swappingService)
    {
        _accountService = accountService;
        _batteryService = batteryService;
        _bookingService = bookingService;
        _paymentService = paymentService;
        _supportRequestService = supportRequestService;
        _swappingService = swappingService;
    }

    [HttpGet("station-context")]
    public async Task<IActionResult> GetStationContext()
    {
        var forbidden = EnsureStaff();
        if (forbidden is not null)
        {
            return forbidden;
        }

        if (CurrentAccountId is not Guid accountId)
        {
            return MissingCurrentAccount();
        }

        var result = await _accountService.GetAccountProfileAsync(accountId);
        return ApiResult(result, "STATION_CONTEXT_FETCHED", "STAFF_STATION_NOT_ASSIGNED");
    }

    [HttpGet("bookings")]
    public async Task<IActionResult> GetBookings()
    {
        var forbidden = EnsureStaff();
        if (forbidden is not null)
        {
            return forbidden;
        }

        if (CurrentAccountId is not Guid accountId)
        {
            return MissingCurrentAccount();
        }

        var result = await _bookingService.GetByStaffStationAsync(accountId);
        return ApiResult(result, "STAFF_BOOKING_LIST_FETCHED", "STAFF_BOOKING_LIST_FAILED");
    }

    [HttpPatch("bookings/{bookingId:guid}/decision")]
    public async Task<IActionResult> DecideBooking(Guid bookingId, [FromBody] StaffBookingDecisionRequest request)
    {
        var forbidden = EnsureStaff();
        if (forbidden is not null)
        {
            return forbidden;
        }

        if (CurrentAccountId is not Guid accountId)
        {
            return MissingCurrentAccount();
        }

        var normalizedDecision = request.Decision?.Trim().ToUpperInvariant() switch
        {
            "APPROVE" => "Approved",
            "APPROVED" => "Approved",
            "REJECT" => "Rejected",
            "REJECTED" => "Rejected",
            _ => request.Decision ?? string.Empty
        };

        var result = await _bookingService.UpdateBookingStatusAsync(bookingId, normalizedDecision, accountId, request.StaffNote);
        return ApiResult(result, "BOOKING_STATUS_UPDATED", "BOOKING_STATUS_UPDATE_FAILED");
    }

    [HttpGet("inventory")]
    public async Task<IActionResult> GetInventory()
    {
        var forbidden = EnsureStaff();
        if (forbidden is not null)
        {
            return forbidden;
        }

        if (CurrentAccountId is not Guid accountId)
        {
            return MissingCurrentAccount();
        }

        var result = await _batteryService.GetBatteriesByStaffStationAsync(accountId);
        return ApiResult(result, "STAFF_INVENTORY_FETCHED", "STAFF_INVENTORY_FAILED");
    }

    [HttpPatch("batteries/{batteryId:guid}/status")]
    public async Task<IActionResult> UpdateBatteryStatus(Guid batteryId, [FromBody] UpdateBattery dto)
    {
        var forbidden = EnsureStaff();
        if (forbidden is not null)
        {
            return forbidden;
        }

        dto.BatteryId = batteryId;
        var result = await _batteryService.UpdateBatteryAsync(dto);
        return ApiResult(result, "BATTERY_STATUS_UPDATED", "BATTERY_STATUS_UPDATE_FAILED");
    }

    [HttpPost("swaps/complete")]
    public async Task<IActionResult> CompleteSwap([FromBody] CompleteSwapRequest request)
    {
        var forbidden = EnsureStaff();
        if (forbidden is not null)
        {
            return forbidden;
        }

        if (CurrentAccountId is not Guid accountId)
        {
            return MissingCurrentAccount();
        }

        var result = await _swappingService.ConfirmAndSwapAsync(new ConfirmSwapDTO
        {
            BookingId = request.BookingId,
            StaffId = accountId,
            Notes = request.Note
        });

        return ApiResult(result, "SWAP_COMPLETED", "SWAP_COMPLETE_FAILED");
    }

    [HttpPost("payments/record")]
    public async Task<IActionResult> RecordPayment([FromBody] RecordStaffPaymentRequest request)
    {
        var forbidden = EnsureStaff();
        if (forbidden is not null)
        {
            return forbidden;
        }

        var result = await _paymentService.CreatePayment(new CreatePaymentDto
        {
            Price = request.Amount,
            Method = request.PaymentMethod ?? "CASH",
            PaymentGateId = 0,
            TransactionId = request.SwapTransactionId
        });

        return ApiResult(result, "PAYMENT_RECORDED", "PAYMENT_RECORD_FAILED");
    }

    [HttpGet("support-requests")]
    public async Task<IActionResult> GetSupportRequests()
    {
        var forbidden = EnsureStaff();
        if (forbidden is not null)
        {
            return forbidden;
        }

        if (CurrentAccountId is not Guid accountId)
        {
            return MissingCurrentAccount();
        }

        var result = await _supportRequestService.GetByStaffIdAsync(accountId);
        return ApiResult(result, "STAFF_SUPPORT_REQUEST_LIST_FETCHED", "STAFF_SUPPORT_REQUEST_LIST_FAILED");
    }

    [HttpPatch("support-requests/{supportRequestId:guid}/response")]
    public async Task<IActionResult> RespondSupportRequest(Guid supportRequestId, [FromBody] RespondSupportRequest request)
    {
        var forbidden = EnsureStaff();
        if (forbidden is not null)
        {
            return forbidden;
        }

        if (CurrentAccountId is not Guid accountId)
        {
            return MissingCurrentAccount();
        }

        var result = await _supportRequestService.UpdateAsync(supportRequestId, new SupportRequestUpdateDTO
        {
            StaffId = accountId,
            ResponseText = request.ResponseMessage
        });

        return ApiResult(result, "SUPPORT_REQUEST_RESPONDED", "SUPPORT_REQUEST_RESPONSE_FAILED");
    }

    private IActionResult? EnsureStaff()
    {
        return string.Equals(CurrentRole, "Staff", StringComparison.OrdinalIgnoreCase)
            ? null
            : Forbidden("Only staff accounts can access this endpoint.", "STAFF_ACCESS_REQUIRED");
    }
}

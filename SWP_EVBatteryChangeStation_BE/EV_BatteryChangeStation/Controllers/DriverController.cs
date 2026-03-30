using EV_BatteryChangeStation_Common.DTOs.BookingDTO;
using EV_BatteryChangeStation_Common.DTOs.CarDTO;
using EV_BatteryChangeStation_Common.DTOs.FeedBackDTO;
using EV_BatteryChangeStation_Common.DTOs.SupportRequestDTO;
using EV_BatteryChangeStation.Contracts.Bookings;
using EV_BatteryChangeStation.Contracts.Feedback;
using EV_BatteryChangeStation.Contracts.Support;
using EV_BatteryChangeStation_Service.InternalService.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EV_BatteryChangeStation.Controllers;

[Authorize]
[Route("api/v1/driver")]
public sealed class DriverController : ApiControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly ICarService _carService;
    private readonly IPaymentService _paymentService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly ISupportRequestService _supportRequestService;
    private readonly ISwappingService _swappingService;
    private readonly IFeedBackService _feedBackService;

    public DriverController(
        IBookingService bookingService,
        ICarService carService,
        IPaymentService paymentService,
        ISubscriptionService subscriptionService,
        ISupportRequestService supportRequestService,
        ISwappingService swappingService,
        IFeedBackService feedBackService)
    {
        _bookingService = bookingService;
        _carService = carService;
        _paymentService = paymentService;
        _subscriptionService = subscriptionService;
        _supportRequestService = supportRequestService;
        _swappingService = swappingService;
        _feedBackService = feedBackService;
    }

    [HttpGet("vehicles")]
    public async Task<IActionResult> GetVehicles()
    {
        var forbidden = EnsureDriver();
        if (forbidden is not null)
        {
            return forbidden;
        }

        if (CurrentAccountId is not Guid accountId)
        {
            return MissingCurrentAccount();
        }

        var result = await _carService.GetCarsByOwnerIdAsync(accountId);
        return ApiResult(result, "VEHICLE_LIST_FETCHED", "VEHICLE_LIST_FAILED");
    }

    [HttpPost("vehicles")]
    public async Task<IActionResult> CreateVehicle([FromBody] CreateCarDto dto)
    {
        var forbidden = EnsureDriver();
        if (forbidden is not null)
        {
            return forbidden;
        }

        if (CurrentAccountId is not Guid accountId)
        {
            return MissingCurrentAccount();
        }

        dto.OwnerId = accountId;
        var result = await _carService.AddCarAsync(dto);
        return ApiResult(result, "VEHICLE_LINKED", "VEHICLE_CREATE_FAILED");
    }

    [HttpGet("bookings")]
    public async Task<IActionResult> GetBookings()
    {
        var forbidden = EnsureDriver();
        if (forbidden is not null)
        {
            return forbidden;
        }

        if (CurrentAccountId is not Guid accountId)
        {
            return MissingCurrentAccount();
        }

        var result = await _bookingService.GetByAccountIdAsync(accountId);
        return ApiResult(result, "BOOKING_LIST_FETCHED", "BOOKING_LIST_FAILED");
    }

    [HttpGet("bookings/{bookingId:guid}")]
    public async Task<IActionResult> GetBooking(Guid bookingId)
    {
        var forbidden = EnsureDriver();
        if (forbidden is not null)
        {
            return forbidden;
        }

        if (CurrentAccountId is not Guid accountId)
        {
            return MissingCurrentAccount();
        }

        var result = await _bookingService.GetByIdForAccountAsync(bookingId, accountId);
        return ApiResult(result, "BOOKING_DETAIL_FETCHED", "BOOKING_NOT_FOUND");
    }

    [HttpPost("bookings")]
    public async Task<IActionResult> CreateBooking([FromBody] CreateDriverBookingRequest request)
    {
        var forbidden = EnsureDriver();
        if (forbidden is not null)
        {
            return forbidden;
        }

        if (CurrentAccountId is not Guid accountId)
        {
            return MissingCurrentAccount();
        }

        var result = await _bookingService.CreateAsync(new BookingCreateDTO
        {
            AccountId = accountId,
            StationId = request.StationId,
            VehicleId = request.VehicleId,
            DateTime = request.BookingTime,
            Notes = request.Note,
            IsApproved = "Pending"
        });

        return ApiResult(result, "BOOKING_CREATED", "BOOKING_CREATE_FAILED");
    }

    [HttpGet("subscriptions/current")]
    public async Task<IActionResult> GetCurrentSubscription()
    {
        var forbidden = EnsureDriver();
        if (forbidden is not null)
        {
            return forbidden;
        }

        if (CurrentAccountId is not Guid accountId)
        {
            return MissingCurrentAccount();
        }

        var result = await _subscriptionService.GetActiveByAccountIdAsync(accountId);
        return ApiResult(result, "SUBSCRIPTION_FETCHED", "SUBSCRIPTION_NOT_FOUND");
    }

    [HttpGet("payments")]
    public async Task<IActionResult> GetPayments()
    {
        var forbidden = EnsureDriver();
        if (forbidden is not null)
        {
            return forbidden;
        }

        if (CurrentAccountId is not Guid accountId)
        {
            return MissingCurrentAccount();
        }

        var result = await _paymentService.GetPaymentByAccountId(accountId);
        return ApiResult(result, "PAYMENT_HISTORY_FETCHED", "PAYMENT_HISTORY_FAILED");
    }

    [HttpGet("swaps/history")]
    public async Task<IActionResult> GetSwapHistory([FromQuery] Guid vehicleId)
    {
        var forbidden = EnsureDriver();
        if (forbidden is not null)
        {
            return forbidden;
        }

        if (vehicleId == Guid.Empty)
        {
            return BadRequest(new
            {
                success = false,
                code = "VEHICLE_ID_REQUIRED",
                message = "vehicleId query parameter is required."
            });
        }

        var result = await _swappingService.GetTransactionByCarIdAsync(vehicleId);
        return ApiResult(result, "SWAP_HISTORY_FETCHED", "SWAP_HISTORY_FAILED");
    }

    [HttpGet("support-requests")]
    public async Task<IActionResult> GetSupportRequests()
    {
        var forbidden = EnsureDriver();
        if (forbidden is not null)
        {
            return forbidden;
        }

        if (CurrentAccountId is not Guid accountId)
        {
            return MissingCurrentAccount();
        }

        var result = await _supportRequestService.GetByAccountIdAsync(accountId);
        return ApiResult(result, "SUPPORT_REQUEST_LIST_FETCHED", "SUPPORT_REQUEST_LIST_FAILED");
    }

    [HttpPost("support-requests")]
    public async Task<IActionResult> CreateSupportRequest([FromBody] CreateSupportRequest request)
    {
        var forbidden = EnsureDriver();
        if (forbidden is not null)
        {
            return forbidden;
        }

        if (CurrentAccountId is not Guid accountId)
        {
            return MissingCurrentAccount();
        }

        var result = await _supportRequestService.CreateAsync(new SupportRequestCreateDTO
        {
            AccountId = accountId,
            IssueType = request.IssueType,
            Description = request.Description
        });

        return ApiResult(result, "SUPPORT_REQUEST_CREATED", "SUPPORT_REQUEST_CREATE_FAILED");
    }

    [HttpPost("feedback")]
    public async Task<IActionResult> CreateFeedback([FromBody] CreateDriverFeedbackRequest request)
    {
        var forbidden = EnsureDriver();
        if (forbidden is not null)
        {
            return forbidden;
        }

        if (CurrentAccountId is not Guid accountId)
        {
            return MissingCurrentAccount();
        }

        var result = await _feedBackService.CreateAsync(new CreateFeedBackDTO
        {
            AccountId = accountId,
            BookingId = request.BookingId,
            Rating = request.Rating,
            Comment = request.Comment ?? string.Empty
        });

        return ApiResult(result, "FEEDBACK_CREATED", "FEEDBACK_CREATE_FAILED");
    }

    private IActionResult? EnsureDriver()
    {
        if (string.Equals(CurrentRole, "Customer", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(CurrentRole, "Driver", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return Forbidden("Only driver/customer accounts can access this endpoint.", "DRIVER_ACCESS_REQUIRED");
    }
}

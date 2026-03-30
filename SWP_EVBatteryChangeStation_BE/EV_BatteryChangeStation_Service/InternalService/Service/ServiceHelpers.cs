using EV_BatteryChangeStation_Common.DTOs.AccountDto;
using EV_BatteryChangeStation_Common.DTOs.BatteryDTO;
using EV_BatteryChangeStation_Common.DTOs.BookingDTO;
using EV_BatteryChangeStation_Common.DTOs.CarDTO;
using EV_BatteryChangeStation_Common.DTOs.FeedBackDTO;
using EV_BatteryChangeStation_Common.DTOs.PaymentDTO;
using EV_BatteryChangeStation_Common.DTOs.RoleDTO;
using EV_BatteryChangeStation_Common.DTOs.StationDTO;
using EV_BatteryChangeStation_Common.DTOs.SubscriptionDTO;
using EV_BatteryChangeStation_Common.DTOs.SupportRequestDTO;
using EV_BatteryChangeStation_Common.DTOs.SwappingtransactionDto;
using EV_BatteryChangeStation_Repository.Entities;
using EV_BatteryChangeStation_Service.Base;

namespace EV_BatteryChangeStation_Service.InternalService.Service;

internal static class ServiceResponse
{
    public static ServiceResult Ok(string message, object? data = null)
        => data is null ? new ServiceResult(200, message) : new ServiceResult(200, message, data);

    public static ServiceResult Created(string message, object? data = null)
        => data is null ? new ServiceResult(201, message) : new ServiceResult(201, message, data);

    public static ServiceResult BadRequest(string message, params string[] errors)
        => errors.Length == 0 ? new ServiceResult(400, message) : new ServiceResult(400, message, errors.ToList());

    public static ServiceResult Unauthorized(string message)
        => new(401, message);

    public static ServiceResult Forbidden(string message)
        => new(403, message);

    public static ServiceResult NotFound(string message)
        => new(404, message);

    public static ServiceResult Conflict(string message)
        => new(409, message);

    public static ServiceResult Error(string message)
        => new(500, message);
}

internal static class LegacyStatusMapper
{
    public static bool ToBool(string? status, params string[] truthyStatuses)
        => truthyStatuses.Any(x => string.Equals(x, status, StringComparison.OrdinalIgnoreCase));

    public static string ToActiveStatus(bool? value, string trueStatus = "ACTIVE", string falseStatus = "INACTIVE")
        => value == false ? falseStatus : trueStatus;
}

internal static class LegacyDtoMapper
{
    public static ViewAccountDTOs ToViewDto(this Account account, Guid? stationId = null)
    {
        return new ViewAccountDTOs
        {
            AccountId = account.AccountId,
            RoleId = account.RoleId,
            AccountName = account.Username,
            Email = account.Email,
            FullName = account.FullName,
            Password = string.Empty,
            Gender = account.Gender,
            Address = account.Address,
            PhoneNumber = account.PhoneNumber,
            DateOfBirth = account.DateOfBirth,
            Status = LegacyStatusMapper.ToBool(account.Status, "ACTIVE"),
            CreateDate = account.CreateDate,
            UpdateDate = account.UpdateDate
        };
    }

    public static ViewRoleDto ToViewDto(this Role role)
    {
        return new ViewRoleDto
        {
            RoleId = role.RoleId,
            RoleName = role.RoleName,
            Status = LegacyStatusMapper.ToBool(role.Status, "ACTIVE"),
            CreateDate = role.CreateDate,
            UpdateDate = role.UpdateDate
        };
    }

    public static StationDTO ToDto(this Station station)
    {
        return new StationDTO
        {
            StationId = station.StationId,
            StationName = station.StationName,
            Address = station.Address,
            PhoneNumber = station.PhoneNumber,
            Status = LegacyStatusMapper.ToBool(station.Status, "ACTIVE"),
            BatteryQuantity = station.CurrentBatteryCount
        };
    }

    public static ViewCarDTO ToDto(this Vehicle vehicle)
    {
        return new ViewCarDTO
        {
            VehicleId = vehicle.VehicleId,
            OwnerId = vehicle.OwnerId,
            Vin = vehicle.Vin,
            LicensePlate = vehicle.LicensePlate,
            CurrentBatteryId = vehicle.CurrentBatteryId,
            Model = vehicle.Model?.ModelName ?? string.Empty,
            BatteryType = vehicle.Model?.BatteryType?.BatteryTypeName
                ?? vehicle.Model?.BatteryType?.BatteryTypeCode
                ?? string.Empty,
            Producer = vehicle.Model?.Producer ?? string.Empty,
            CreateDate = vehicle.CreateDate,
            Images = null,
            Status = vehicle.Status
        };
    }

    public static ViewBatteryDTO ToDto(this Battery battery)
    {
        return new ViewBatteryDTO
        {
            BatteryId = battery.BatteryId,
            Capacity = battery.CapacityKwh,
            LastUsed = battery.LastUsedAt,
            Status = LegacyStatusMapper.ToBool(battery.Status, "AVAILABLE", "CHARGING", "IN_VEHICLE"),
            StateOfHealth = battery.StateOfHealth,
            PercentUse = battery.CurrentChargeLevel,
            TypeBattery = battery.BatteryType?.BatteryTypeName ?? battery.BatteryType?.BatteryTypeCode ?? string.Empty,
            BatterySwapDate = battery.LastChargedAt,
            InsuranceDate = battery.InsuranceDate,
            StationId = battery.StationId ?? Guid.Empty
        };
    }

    public static BookingDTO ToDto(this Booking booking)
    {
        return new BookingDTO
        {
            BookingId = booking.BookingId,
            AccountId = booking.AccountId,
            VehicleId = booking.VehicleId,
            StationId = booking.StationId,
            BatteryId = booking.RequestedBatteryTypeId,
            DateTime = booking.TargetTime,
            CreatedDate = booking.CreateDate,
            Notes = booking.Notes,
            IsApproved = booking.Status
        };
    }

    public static object ToDetailDto(this Booking booking)
    {
        return new
        {
            booking.BookingId,
            booking.AccountId,
            booking.VehicleId,
            booking.StationId,
            requestedBatteryTypeId = booking.RequestedBatteryTypeId,
            requestedBatteryTypeName = booking.RequestedBatteryType?.BatteryTypeName,
            stationName = booking.Station?.StationName,
            vehicleModel = booking.Vehicle?.Model?.ModelName,
            vehicleLicensePlate = booking.Vehicle?.LicensePlate,
            booking.TargetTime,
            booking.Status,
            booking.Notes,
            booking.StaffNote,
            booking.ApprovedBy,
            booking.ApprovedDate,
            booking.CreateDate,
            swapTransactionId = booking.SwappingTransaction?.TransactionId
        };
    }

    public static SubscriptionViewDTO ToDto(this SubscriptionPlan plan)
    {
        return new SubscriptionViewDTO
        {
            SubscriptionId = plan.PlanId,
            Name = plan.PlanName,
            Price = plan.BasePrice,
            ExtraFee = plan.ExtraFeePerSwap,
            Description = plan.Description,
            DurationPackage = plan.DurationDays,
            IsActive = LegacyStatusMapper.ToBool(plan.Status, "ACTIVE"),
            CreateDate = plan.CreateDate,
            UpdateDate = plan.UpdateDate,
            AccountId = null
        };
    }

    public static object ToActiveSubscriptionDto(this UserSubscription subscription)
    {
        return new
        {
            subscription.UserSubscriptionId,
            subscription.AccountId,
            subscription.VehicleId,
            subscription.PlanId,
            planName = subscription.Plan?.PlanName,
            price = subscription.Plan?.BasePrice,
            extraFee = subscription.Plan?.ExtraFeePerSwap,
            durationPackage = subscription.Plan?.DurationDays,
            subscription.StartDate,
            subscription.EndDate,
            subscription.RemainingSwaps,
            subscription.AutoRenew,
            subscription.Status,
            vehicleModel = subscription.Vehicle?.Model?.ModelName,
            batteryType = subscription.Vehicle?.Model?.BatteryType?.BatteryTypeName
        };
    }

    public static SupportRequestViewDTO ToDto(this SupportRequest request)
    {
        var latestResponse = request.Responses
            .OrderByDescending(x => x.RespondedAt)
            .FirstOrDefault();

        return new SupportRequestViewDTO
        {
            RequestId = request.RequestId,
            IssueType = request.IssueType,
            Description = request.Description,
            CreateDate = request.CreateDate,
            Status = LegacyStatusMapper.ToBool(request.Status, "OPEN", "IN_PROGRESS"),
            AccountId = request.AccountId,
            StaffId = latestResponse?.StaffId,
            ResponseText = latestResponse?.ResponseMessage,
            ResponseDate = latestResponse?.RespondedAt
        };
    }

    public static ViewPaymentDto ToDto(this Payment payment)
    {
        return new ViewPaymentDto
        {
            PaymentId = payment.PaymentId,
            Price = payment.Amount,
            Method = payment.PaymentMethod,
            Status = LegacyStatusMapper.ToBool(payment.Status, "PAID", "SUCCESS", "COMPLETED"),
            CreateDate = payment.PaidAt ?? payment.CreateDate,
            SubscriptionId = payment.UserSubscriptionId ?? Guid.Empty,
            TransactionId = payment.TransactionId ?? Guid.Empty
        };
    }

    public static PaymentRespondDto ToRespondDto(this Payment payment)
    {
        return new PaymentRespondDto
        {
            PaymentId = payment.PaymentId,
            Price = payment.Amount,
            Method = payment.PaymentMethod,
            Status = payment.Status,
            PaymentGateId = payment.PaymentGatewayId,
            CreateDate = payment.PaidAt ?? payment.CreateDate,
            SubscriptionId = payment.UserSubscriptionId,
            TransactionId = payment.TransactionId,
            AccountId = payment.AccountId
        };
    }

    public static ViewSwappingDto ToDto(this SwappingTransaction transaction)
    {
        return new ViewSwappingDto
        {
            TransactionId = transaction.TransactionId,
            Notes = transaction.Notes,
            StaffId = transaction.StaffId,
            VehicleId = transaction.VehicleId,
            NewBatteryId = transaction.ReleasedBatteryId,
            Status = "COMPLETED",
            CreateDate = transaction.CreateDate
        };
    }

    public static FeedBackDTO ToDto(this Feedback feedback)
    {
        return new FeedBackDTO
        {
            FeedbackId = feedback.FeedbackId,
            Rating = feedback.Rating,
            Comment = feedback.Comment ?? string.Empty,
            CreateDate = feedback.CreateDate,
            AccountId = feedback.AccountId,
            BookingId = feedback.BookingId ?? Guid.Empty
        };
    }
}

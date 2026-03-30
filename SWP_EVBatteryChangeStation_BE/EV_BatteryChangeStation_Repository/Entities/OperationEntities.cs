using System;
using System.Collections.Generic;

namespace EV_BatteryChangeStation_Repository.Entities;

public class Booking
{
    public Guid BookingId { get; set; }
    public Guid AccountId { get; set; }
    public Guid VehicleId { get; set; }
    public Guid StationId { get; set; }
    public Guid RequestedBatteryTypeId { get; set; }
    public DateTime TargetTime { get; set; }
    public string Status { get; set; } = "PENDING";
    public string? Notes { get; set; }
    public string? StaffNote { get; set; }
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public DateTime? CancelledDate { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }

    public Account? Account { get; set; }
    public Vehicle? Vehicle { get; set; }
    public Station? Station { get; set; }
    public BatteryType? RequestedBatteryType { get; set; }
    public Account? ApprovedByAccount { get; set; }
    public SwappingTransaction? SwappingTransaction { get; set; }
    public ICollection<BatteryReturnInspection> BatteryReturnInspections { get; set; } = new List<BatteryReturnInspection>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<SupportRequest> SupportRequests { get; set; } = new List<SupportRequest>();
    public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}

public class SwappingTransaction
{
    public Guid TransactionId { get; set; }
    public Guid BookingId { get; set; }
    public Guid VehicleId { get; set; }
    public Guid StaffId { get; set; }
    public Guid StationId { get; set; }
    public Guid ReturnedBatteryId { get; set; }
    public decimal? ReturnedBatterySoH { get; set; }
    public decimal? ReturnedBatteryCharge { get; set; }
    public string? ReturnedBatteryCondition { get; set; }
    public Guid ReleasedBatteryId { get; set; }
    public decimal? ReleasedBatterySoH { get; set; }
    public decimal? ReleasedBatteryCharge { get; set; }
    public decimal SwapFee { get; set; }
    public bool UsedSubscription { get; set; }
    public string? Notes { get; set; }
    public DateTime CreateDate { get; set; }

    public Booking? Booking { get; set; }
    public Vehicle? Vehicle { get; set; }
    public Account? Staff { get; set; }
    public Station? Station { get; set; }
    public Battery? ReturnedBattery { get; set; }
    public Battery? ReleasedBattery { get; set; }
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<SupportRequest> SupportRequests { get; set; } = new List<SupportRequest>();
    public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}

public class BatteryReturnInspection
{
    public Guid InspectionId { get; set; }
    public Guid BookingId { get; set; }
    public Guid BatteryId { get; set; }
    public Guid StationId { get; set; }
    public Guid StaffId { get; set; }
    public decimal? SoHPercent { get; set; }
    public string PhysicalCondition { get; set; } = string.Empty;
    public string? InspectionNote { get; set; }
    public string NextStatus { get; set; } = string.Empty;
    public DateTime CreateDate { get; set; }

    public Booking? Booking { get; set; }
    public Battery? Battery { get; set; }
    public Station? Station { get; set; }
    public Account? Staff { get; set; }
}

public class Payment
{
    public Guid PaymentId { get; set; }
    public Guid AccountId { get; set; }
    public Guid? BookingId { get; set; }
    public Guid? UserSubscriptionId { get; set; }
    public Guid? TransactionId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "VND";
    public string PaymentType { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = "PENDING";
    public string? TransactionReference { get; set; }
    public long? PaymentGatewayId { get; set; }
    public Guid? RecordedByAccountId { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime CreateDate { get; set; }

    public Account? Account { get; set; }
    public Booking? Booking { get; set; }
    public UserSubscription? UserSubscription { get; set; }
    public SwappingTransaction? Transaction { get; set; }
    public Account? RecordedByAccount { get; set; }
}

public class SupportRequest
{
    public Guid RequestId { get; set; }
    public Guid AccountId { get; set; }
    public Guid? StationId { get; set; }
    public Guid? BookingId { get; set; }
    public Guid? TransactionId { get; set; }
    public string IssueType { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Priority { get; set; } = "MEDIUM";
    public string Status { get; set; } = "OPEN";
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public DateTime? ClosedDate { get; set; }

    public Account? Account { get; set; }
    public Station? Station { get; set; }
    public Booking? Booking { get; set; }
    public SwappingTransaction? Transaction { get; set; }
    public ICollection<SupportRequestResponse> Responses { get; set; } = new List<SupportRequestResponse>();
}

public class SupportRequestResponse
{
    public Guid ResponseId { get; set; }
    public Guid RequestId { get; set; }
    public Guid StaffId { get; set; }
    public string ResponseMessage { get; set; } = string.Empty;
    public string StatusAfterResponse { get; set; } = string.Empty;
    public DateTime RespondedAt { get; set; }

    public SupportRequest? Request { get; set; }
    public Account? Staff { get; set; }
}

public class Feedback
{
    public Guid FeedbackId { get; set; }
    public Guid AccountId { get; set; }
    public Guid StationId { get; set; }
    public Guid? BookingId { get; set; }
    public Guid? TransactionId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreateDate { get; set; }

    public Account? Account { get; set; }
    public Station? Station { get; set; }
    public Booking? Booking { get; set; }
    public SwappingTransaction? Transaction { get; set; }
}


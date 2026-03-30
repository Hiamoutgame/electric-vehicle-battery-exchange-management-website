using System;
using System.Collections.Generic;

namespace EV_BatteryChangeStation_Repository.Entities;

public class StationStaffAssignment
{
    public Guid AssignmentId { get; set; }
    public Guid StaffId { get; set; }
    public Guid StationId { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
    public string Status { get; set; } = "ACTIVE";
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }

    public Account? Staff { get; set; }
    public Station? Station { get; set; }
}

public class VehicleModel
{
    public Guid ModelId { get; set; }
    public string ModelName { get; set; } = string.Empty;
    public string? Producer { get; set; }
    public Guid BatteryTypeId { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = "ACTIVE";
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }

    public BatteryType? BatteryType { get; set; }
    public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}

public class Vehicle
{
    public Guid VehicleId { get; set; }
    public string? Vin { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public Guid? ModelId { get; set; }
    public Guid OwnerId { get; set; }
    public Guid? CurrentBatteryId { get; set; }
    public string Status { get; set; } = "ACTIVE";
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }

    public VehicleModel? Model { get; set; }
    public Account? Owner { get; set; }
    public Battery? CurrentBattery { get; set; }
    public ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<SwappingTransaction> SwappingTransactions { get; set; } = new List<SwappingTransaction>();
    public ICollection<BatteryHistory> BatteryHistoryFromVehicles { get; set; } = new List<BatteryHistory>();
    public ICollection<BatteryHistory> BatteryHistoryToVehicles { get; set; } = new List<BatteryHistory>();
}

public class SubscriptionPlan
{
    public Guid PlanId { get; set; }
    public string PlanCode { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public string Currency { get; set; } = "VND";
    public int? SwapLimitPerMonth { get; set; }
    public int DurationDays { get; set; }
    public decimal? ExtraFeePerSwap { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = "ACTIVE";
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }

    public ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
}

public class UserSubscription
{
    public Guid UserSubscriptionId { get; set; }
    public Guid AccountId { get; set; }
    public Guid VehicleId { get; set; }
    public Guid PlanId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? RemainingSwaps { get; set; }
    public string Status { get; set; } = "ACTIVE";
    public bool AutoRenew { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }

    public Account? Account { get; set; }
    public Vehicle? Vehicle { get; set; }
    public SubscriptionPlan? Plan { get; set; }
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}


using System;
using System.Collections.Generic;

namespace EV_BatteryChangeStation_Repository.Entities;

public class Station
{
    public Guid StationId { get; set; }
    public string StationName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Area { get; set; }
    public string? PhoneNumber { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? OperatingHours { get; set; }
    public string Status { get; set; } = "ACTIVE";
    public int? MaxCapacity { get; set; }
    public int CurrentBatteryCount { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }

    public ICollection<StationBatteryType> StationBatteryTypes { get; set; } = new List<StationBatteryType>();
    public ICollection<StationStaffAssignment> StaffAssignments { get; set; } = new List<StationStaffAssignment>();
    public ICollection<Battery> Batteries { get; set; } = new List<Battery>();
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<SwappingTransaction> SwappingTransactions { get; set; } = new List<SwappingTransaction>();
    public ICollection<BatteryReturnInspection> BatteryReturnInspections { get; set; } = new List<BatteryReturnInspection>();
    public ICollection<StationInventoryLog> InventoryLogs { get; set; } = new List<StationInventoryLog>();
    public ICollection<SupportRequest> SupportRequests { get; set; } = new List<SupportRequest>();
    public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}

public class BatteryType
{
    public Guid BatteryTypeId { get; set; }
    public string BatteryTypeCode { get; set; } = string.Empty;
    public string BatteryTypeName { get; set; } = string.Empty;
    public decimal? Voltage { get; set; }
    public decimal? CapacityKwh { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = "ACTIVE";
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }

    public ICollection<StationBatteryType> StationBatteryTypes { get; set; } = new List<StationBatteryType>();
    public ICollection<VehicleModel> VehicleModels { get; set; } = new List<VehicleModel>();
    public ICollection<Battery> Batteries { get; set; } = new List<Battery>();
    public ICollection<Booking> RequestedBookings { get; set; } = new List<Booking>();
    public ICollection<StationInventoryLog> InventoryLogs { get; set; } = new List<StationInventoryLog>();
}

public class StationBatteryType
{
    public Guid StationBatteryTypeId { get; set; }
    public Guid StationId { get; set; }
    public Guid BatteryTypeId { get; set; }
    public string Status { get; set; } = "ACTIVE";
    public DateTime CreateDate { get; set; }

    public Station? Station { get; set; }
    public BatteryType? BatteryType { get; set; }
}

public class Battery
{
    public Guid BatteryId { get; set; }
    public string SerialNumber { get; set; } = string.Empty;
    public Guid BatteryTypeId { get; set; }
    public decimal? CapacityKwh { get; set; }
    public decimal? StateOfHealth { get; set; }
    public decimal? CurrentChargeLevel { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid? StationId { get; set; }
    public DateOnly? InsuranceDate { get; set; }
    public DateTime? LastChargedAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }

    public BatteryType? BatteryType { get; set; }
    public Station? Station { get; set; }
    public ICollection<Vehicle> VehiclesUsingAsCurrent { get; set; } = new List<Vehicle>();
    public ICollection<BatteryHistory> BatteryHistories { get; set; } = new List<BatteryHistory>();
    public ICollection<SwappingTransaction> ReturnedSwapTransactions { get; set; } = new List<SwappingTransaction>();
    public ICollection<SwappingTransaction> ReleasedSwapTransactions { get; set; } = new List<SwappingTransaction>();
    public ICollection<BatteryReturnInspection> ReturnInspections { get; set; } = new List<BatteryReturnInspection>();
}

public class BatteryHistory
{
    public Guid HistoryId { get; set; }
    public Guid BatteryId { get; set; }
    public Guid? FromStationId { get; set; }
    public Guid? ToStationId { get; set; }
    public Guid? FromVehicleId { get; set; }
    public Guid? ToVehicleId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string? FromStatus { get; set; }
    public string? ToStatus { get; set; }
    public DateTime EventDate { get; set; }
    public decimal? SoHAtTime { get; set; }
    public decimal? ChargeLevelAtTime { get; set; }
    public string? Note { get; set; }
    public Guid? ActorAccountId { get; set; }

    public Battery? Battery { get; set; }
    public Station? FromStation { get; set; }
    public Station? ToStation { get; set; }
    public Vehicle? FromVehicle { get; set; }
    public Vehicle? ToVehicle { get; set; }
    public Account? ActorAccount { get; set; }
}

public class StationInventoryLog
{
    public Guid LogId { get; set; }
    public Guid StationId { get; set; }
    public Guid? BatteryTypeId { get; set; }
    public DateTime LogTime { get; set; }
    public int AvailableBatteries { get; set; }
    public int ReservedBatteries { get; set; }
    public int ChargingBatteries { get; set; }
    public int InVehicleBatteries { get; set; }
    public int MaintenanceBatteries { get; set; }
    public decimal? AvgChargeLevel { get; set; }

    public Station? Station { get; set; }
    public BatteryType? BatteryType { get; set; }
}


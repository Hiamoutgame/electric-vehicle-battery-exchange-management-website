namespace EV_BatteryChangeStation_Repository.Models;

public sealed record StationInventorySummary(
    Guid StationId,
    string StationName,
    int AvailableCount,
    int ReservedCount,
    int ChargingCount,
    int InVehicleCount,
    int MaintenanceCount,
    int FaultyCount,
    int TotalCount);



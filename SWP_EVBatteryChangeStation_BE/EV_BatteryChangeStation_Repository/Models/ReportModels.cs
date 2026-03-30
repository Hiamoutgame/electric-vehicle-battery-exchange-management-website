namespace EV_BatteryChangeStation_Repository.Models;

public sealed record RevenueReportItem(
    string Label,
    decimal Revenue,
    Guid? StationId = null,
    string? StationName = null);

public sealed record PeakHourDemandItem(
    int Hour,
    int BookingCount);

public sealed record StationSwapCountItem(
    Guid StationId,
    string StationName,
    int SwapCount);

public sealed record DailySwapCountItem(
    DateOnly Date,
    int SwapCount,
    Guid StationId,
    string StationName);

public sealed record StationInventoryForecastInput(
    Guid StationId,
    string StationName,
    DateTime LogTime,
    int AvailableBatteries,
    int ReservedBatteries,
    int ChargingBatteries,
    int InVehicleBatteries,
    int MaintenanceBatteries,
    decimal? AvgChargeLevel);



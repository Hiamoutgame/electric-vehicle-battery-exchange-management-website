namespace EV_BatteryChangeStation.Contracts.Bookings;

public sealed class StaffBookingDecisionRequest
{
    public string? Decision { get; init; }

    public string? StaffNote { get; init; }
}

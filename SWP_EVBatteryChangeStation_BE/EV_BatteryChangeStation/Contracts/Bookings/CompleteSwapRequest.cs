namespace EV_BatteryChangeStation.Contracts.Bookings;

public sealed class CompleteSwapRequest
{
    public Guid BookingId { get; init; }

    public string? Note { get; init; }
}

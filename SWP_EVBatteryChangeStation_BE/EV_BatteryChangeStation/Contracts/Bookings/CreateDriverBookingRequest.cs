namespace EV_BatteryChangeStation.Contracts.Bookings;

public sealed class CreateDriverBookingRequest
{
    public Guid StationId { get; init; }

    public Guid VehicleId { get; init; }

    public DateTime BookingTime { get; init; }

    public string? Note { get; init; }
}

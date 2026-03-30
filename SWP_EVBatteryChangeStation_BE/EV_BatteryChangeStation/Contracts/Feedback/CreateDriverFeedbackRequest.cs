namespace EV_BatteryChangeStation.Contracts.Feedback;

public sealed class CreateDriverFeedbackRequest
{
    public Guid BookingId { get; init; }

    public int? Rating { get; init; }

    public string? Comment { get; init; }
}

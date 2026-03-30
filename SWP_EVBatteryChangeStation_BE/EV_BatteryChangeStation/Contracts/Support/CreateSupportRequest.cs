namespace EV_BatteryChangeStation.Contracts.Support;

public sealed class CreateSupportRequest
{
    public string? IssueType { get; init; }

    public string? Description { get; init; }
}

namespace EV_BatteryChangeStation.Contracts.Auth;

public sealed class LoginRequest
{
    public string? Email { get; init; }

    public string? Keyword { get; init; }

    public string? Password { get; init; }
}

using System.Text.Json.Serialization;

namespace EV_BatteryChangeStation.Contracts.Common;

public sealed class ApiFieldError
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Field { get; init; }

    public string Message { get; init; } = string.Empty;
}

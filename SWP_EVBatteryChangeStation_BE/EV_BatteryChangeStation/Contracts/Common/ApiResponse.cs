using System.Text.Json.Serialization;

namespace EV_BatteryChangeStation.Contracts.Common;

public sealed class ApiResponse
{
    public bool Success { get; init; }

    public string Code { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Data { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyCollection<ApiFieldError>? Errors { get; init; }
}

using System.Text.Json;

namespace DeviceCommunicator;

public interface IDeviceCommunicator
{
    event EventHandler<DeviceStatusMessage>? StatusReceived;
    event EventHandler<DeviceTelemetryMessage>? TelemetryReceived;
    event EventHandler<DeviceLwtMessage>? LwtReceived;

    Task<CommandResult> TurnOnAsync(string externalId, CancellationToken cancellationToken = default);
    Task<CommandResult> TurnOffAsync(string externalId, CancellationToken cancellationToken = default);
    Task<CommandResult> GetSensorValueAsync(string externalId, CancellationToken cancellationToken = default);
    Task<CommandResult> GetSensorStatusAsync(string externalId, CancellationToken cancellationToken = default);
}

public sealed record CommandResult
{
    public required bool Success { get; init; }
    public required string CommandId { get; init; }
    public required string Command { get; init; }
    public string? Error { get; init; }
    public JsonElement? Data { get; init; }
    public DateTimeOffset Timestamp { get; init; }

    public static CommandResult Failed(string commandId, string command, string error) => new()
    {
        Success = false,
        CommandId = commandId,
        Command = command,
        Error = error,
        Timestamp = DateTimeOffset.UtcNow
    };
}

public sealed record DeviceStatusMessage
{
    public required string DeviceId { get; init; }
    public required string Status { get; init; }
    public bool Alarm { get; init; }
    public DateTimeOffset Timestamp { get; init; }
}

public sealed record DeviceTelemetryMessage
{
    public required string DeviceId { get; init; }
    public JsonElement Data { get; init; }
    public DateTimeOffset Timestamp { get; init; }
}

public sealed record DeviceLwtMessage
{
    public required string DeviceId { get; init; }
    public required string Status { get; init; }
    public DateTimeOffset Timestamp { get; init; }
}

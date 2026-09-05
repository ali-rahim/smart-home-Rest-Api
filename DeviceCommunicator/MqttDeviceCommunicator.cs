using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using MQTTnet;
using MQTTnet.Protocol;

namespace DeviceCommunicator;

public sealed class MqttDeviceCommunicator : IDeviceCommunicator, IHostedService, IAsyncDisposable
{
    private readonly IMqttClient _client;
    private readonly ConcurrentDictionary<string, TaskCompletionSource<CommandResult>> _pendingCommands = new();
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    private readonly CancellationTokenSource _lifetimeCts = new();

    private Task? _connectionTask;

    private const string BrokerAddress = "localhost";
    private const int BrokerPort = 1883;
    private const int CommandTimeoutSeconds = 10;
    private static readonly TimeSpan ReconnectDelay = TimeSpan.FromSeconds(5);

    public event EventHandler<DeviceStatusMessage>? StatusReceived;
    public event EventHandler<DeviceTelemetryMessage>? TelemetryReceived;
    public event EventHandler<DeviceLwtMessage>? LwtReceived;

    public MqttDeviceCommunicator()
    {
        var factory = new MqttClientFactory();
        _client = factory.CreateMqttClient();
        _client.ApplicationMessageReceivedAsync += OnApplicationMessageReceivedAsync;
        _client.DisconnectedAsync += OnDisconnectedAsync;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _connectionTask = MaintainConnectionAsync(_lifetimeCts.Token);
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _lifetimeCts.Cancel();

        if (_connectionTask is not null)
        {
            try
            {
                await _connectionTask.WaitAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Application shutdown is expected to cancel the connection loop.
            }
        }

        if (_client.IsConnected)
            await _client.DisconnectAsync();
    }

    public async Task<CommandResult> TurnOnAsync(string externalId, CancellationToken cancellationToken = default)
        => await SendCommandAsync(externalId, "turn_on", cancellationToken);

    public async Task<CommandResult> TurnOffAsync(string externalId, CancellationToken cancellationToken = default)
        => await SendCommandAsync(externalId, "turn_off", cancellationToken);

    public async Task<CommandResult> GetSensorValueAsync(string externalId, CancellationToken cancellationToken = default)
        => await SendCommandAsync(externalId, "get_sensor_value", cancellationToken);

    public async Task<CommandResult> GetSensorStatusAsync(string externalId, CancellationToken cancellationToken = default)
        => await SendCommandAsync(externalId, "get_sensor_status", cancellationToken);

    private async Task<CommandResult> SendCommandAsync(
        string deviceId,
        string command,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            throw new ArgumentException("Device ID cannot be empty.", nameof(deviceId));

        await EnsureConnectedAsync(cancellationToken);

        var commandId = Guid.NewGuid().ToString("N");
        var completion = new TaskCompletionSource<CommandResult>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        if (!_pendingCommands.TryAdd(commandId, completion))
            throw new InvalidOperationException("Could not register the MQTT command.");

        try
        {
            var payload = new
            {
                commandId,
                command,
                timestamp = DateTimeOffset.UtcNow
            };

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(GetTopic(deviceId, "command"))
                .WithPayload(JsonSerializer.Serialize(payload))
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            await _client.PublishAsync(message, cancellationToken);

            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(TimeSpan.FromSeconds(CommandTimeoutSeconds));

            try
            {
                return await completion.Task.WaitAsync(timeoutCts.Token);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                return CommandResult.Failed(commandId, command, "Timed out waiting for device feedback.");
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return CommandResult.Failed(commandId, command, "Command was cancelled.");
        }
        catch (Exception ex)
        {
            return CommandResult.Failed(commandId, command, ex.Message);
        }
        finally
        {
            _pendingCommands.TryRemove(commandId, out _);
        }
    }

    private async Task MaintainConnectionAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (!_client.IsConnected)
                    await EnsureConnectedAsync(cancellationToken);

                await Task.Delay(ReconnectDelay, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch
            {
                try
                {
                    await Task.Delay(ReconnectDelay, cancellationToken);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }
    }

    private async Task EnsureConnectedAsync(CancellationToken cancellationToken)
    {
        if (_client.IsConnected)
            return;

        await _connectionLock.WaitAsync(cancellationToken);
        try
        {
            if (_client.IsConnected)
                return;

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(BrokerAddress, BrokerPort)
                .WithClientId($"smart-home-backend-{Guid.NewGuid():N}")
                .WithCleanSession()
                .Build();

            await _client.ConnectAsync(options, cancellationToken);

            await _client.SubscribeAsync(new MqttTopicFilterBuilder()
                .WithTopic("home/+/feedback")
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build(), cancellationToken);

            await _client.SubscribeAsync(new MqttTopicFilterBuilder()
                .WithTopic("home/+/status")
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build(), cancellationToken);

            await _client.SubscribeAsync(new MqttTopicFilterBuilder()
                .WithTopic("home/+/telemetry")
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build(), cancellationToken);

            await _client.SubscribeAsync(new MqttTopicFilterBuilder()
                .WithTopic("home/+/lwt")
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build(), cancellationToken);
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    private Task OnDisconnectedAsync(MqttClientDisconnectedEventArgs args)
    {
        foreach (var pending in _pendingCommands)
        {
            pending.Value.TrySetResult(
                CommandResult.Failed(
                    pending.Key,
                    "unknown",
                    "MQTT connection was lost while waiting for feedback."));
        }

        return Task.CompletedTask;
    }

    private Task OnApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs args)
    {
        var payload = Encoding.UTF8.GetString(args.ApplicationMessage.Payload);
        var topicParts = args.ApplicationMessage.Topic.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (topicParts.Length != 3 || topicParts[0] != "home")
            return Task.CompletedTask;

        var deviceId = topicParts[1];
        var messageType = topicParts[2];

        try
        {
            switch (messageType)
            {
                case "feedback":
                    HandleFeedback(payload);
                    break;

                case "status":
                    HandleStatus(deviceId, payload);
                    break;

                case "telemetry":
                    HandleTelemetry(deviceId, payload);
                    break;

                case "lwt":
                    HandleLwt(deviceId, payload);
                    break;
            }
        }
        catch
        {
            // A malformed MQTT message must not terminate the MQTT receive loop.
        }

        return Task.CompletedTask;
    }

    private void HandleFeedback(string payload)
    {
        var feedback = JsonSerializer.Deserialize<FeedbackPayload>(payload);
        if (feedback is null || string.IsNullOrWhiteSpace(feedback.CommandId))
            return;

        var result = new CommandResult
        {
            Success = feedback.Success,
            CommandId = feedback.CommandId,
            Command = feedback.Command,
            Error = feedback.Error,
            Data = feedback.Data,
            Timestamp = feedback.Timestamp
        };

        if (_pendingCommands.TryGetValue(feedback.CommandId, out var completion))
            completion.TrySetResult(result);
    }

    private void HandleStatus(string deviceId, string payload)
    {
        var status = JsonSerializer.Deserialize<DeviceStatusPayload>(payload);
        if (status is null)
            return;

        StatusReceived?.Invoke(this, new DeviceStatusMessage
        {
            DeviceId = status.DeviceId ?? deviceId,
            Status = status.Status,
            Alarm = status.Alarm,
            Timestamp = status.Timestamp
        });
    }

    private void HandleTelemetry(string deviceId, string payload)
    {
        var telemetry = JsonSerializer.Deserialize<DeviceTelemetryPayload>(payload);
        if (telemetry is null)
            return;

        TelemetryReceived?.Invoke(this, new DeviceTelemetryMessage
        {
            DeviceId = telemetry.DeviceId ?? deviceId,
            Data = telemetry.Data,
            Timestamp = telemetry.Timestamp
        });
    }

    private void HandleLwt(string deviceId, string payload)
    {
        var lwt = JsonSerializer.Deserialize<DeviceLwtPayload>(payload);
        if (lwt is null)
            return;

        LwtReceived?.Invoke(this, new DeviceLwtMessage
        {
            DeviceId = lwt.DeviceId ?? deviceId,
            Status = lwt.Status,
            Timestamp = lwt.Timestamp
        });
    }

    private static string GetTopic(string deviceId, string type)
        => $"home/{deviceId}/{type}";

    public async ValueTask DisposeAsync()
    {
        _client.ApplicationMessageReceivedAsync -= OnApplicationMessageReceivedAsync;
        _client.DisconnectedAsync -= OnDisconnectedAsync;

        _lifetimeCts.Cancel();

        if (_client.IsConnected)
            await _client.DisconnectAsync();

        _connectionLock.Dispose();
        _lifetimeCts.Dispose();
        _client.Dispose();
    }

    private sealed record FeedbackPayload
    {
        public string CommandId { get; init; } = string.Empty;
        public string Command { get; init; } = string.Empty;
        public bool Success { get; init; }
        public string? Error { get; init; }
        public JsonElement? Data { get; init; }
        public DateTimeOffset Timestamp { get; init; }
    }

    private sealed record DeviceStatusPayload
    {
        public string? DeviceId { get; init; }
        public string Status { get; init; } = string.Empty;
        public bool Alarm { get; init; }
        public DateTimeOffset Timestamp { get; init; }
    }

    private sealed record DeviceTelemetryPayload
    {
        public string? DeviceId { get; init; }
        public JsonElement Data { get; init; }
        public DateTimeOffset Timestamp { get; init; }
    }

    private sealed record DeviceLwtPayload
    {
        public string? DeviceId { get; init; }
        public string Status { get; init; } = string.Empty;
        public DateTimeOffset Timestamp { get; init; }
    }
}

using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using EcoAssistant.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace EcoAssistant.API.Services;

public class MqttOptions
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string ClientId { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string TopicFilter { get; set; }
    public bool UseTls { get; set; }
}

public class MqttHostedService : BackgroundService
{
    private readonly ILogger<MqttHostedService> _logger;
    private readonly MqttOptions _options;
    private readonly IServiceScopeFactory _scopeFactory;
    private IMqttClient? _client;

    public MqttHostedService(IOptions<MqttOptions> opts, IServiceScopeFactory scopeFactory, ILogger<MqttHostedService> logger)
    {
        _options = opts.Value;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new MqttFactory();
        _client = factory.CreateMqttClient();

        // Build client options
        var clientBuilder = new MqttClientOptionsBuilder()
            .WithClientId(string.IsNullOrEmpty(_options.ClientId) ? "MyClientId" : _options.ClientId)
            .WithTcpServer(_options.Host, _options.Port)
            .WithCleanSession();

        if (!string.IsNullOrEmpty(_options.Username))
            clientBuilder = clientBuilder.WithCredentials(_options.Username, _options.Password);

        if (_options.UseTls)
            clientBuilder = clientBuilder.WithTls();

        var mqttOptions = clientBuilder.Build();

        // Event: Connected
        _client.ConnectedAsync += async e =>
        {
            _logger.LogInformation("MQTT connected to {Host}:{Port}", _options.Host, _options.Port);

            var subscribeOptions = new MQTTnet.Client.MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(_options.TopicFilter)
                .Build();


            await _client.SubscribeAsync(subscribeOptions, stoppingToken);
            _logger.LogInformation("Subscribed to {Topic}", _options.TopicFilter);
        };

        // Event: Disconnected
        _client.DisconnectedAsync += e =>
        {
            _logger.LogWarning("MQTT disconnected: {Reason}", e.Reason);
            return Task.CompletedTask;
        };

        // Event: Message received
        _client.ApplicationMessageReceivedAsync += async e =>
        {
            try
            {
                var topic = e.ApplicationMessage.Topic ?? string.Empty;
                var payload = e.ApplicationMessage.Payload == null
                    ? "{}"
                    : Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                _logger.LogDebug("MQTT message on {Topic}: {Payload}", topic, payload);

                var parts = topic.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 4 && parts[0] == "devices" && parts[2] == "sensors")
                {
                    if (!int.TryParse(parts[1], out var deviceId) || !int.TryParse(parts[3], out var sensorId))
                    {
                        _logger.LogWarning("Invalid ids in topic {Topic}", topic);
                        return;
                    }

                    var msg = JsonSerializer.Deserialize<MeasureMessage>(payload,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                        ?? new MeasureMessage { Value = 0, Alerted = false };

                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var sensor = await db.Sensors.FirstOrDefaultAsync(s => s.DeviceId == deviceId && s.LocalId == sensorId, stoppingToken);

                    if (sensor == null)
                    {
                        _logger.LogWarning("Sensor not found for device={Device} sensor={Sensor}", deviceId, sensorId);
                        return;
                    }

                    var mesure = new EcoAssistant.Domain.Entities.Mesure
                    {
                        Value = msg.Value,
                        AlertedMesure = msg.Alerted,
                        CreatedAt = msg.CreatedAt ?? DateTimeOffset.UtcNow,
                        SensorLocalId = sensorId,
                        SensorDeviceId =deviceId


                    };

                    var sensorIdProp = typeof(EcoAssistant.Domain.Entities.Mesure).GetProperty("SensorId");
                    if (sensorIdProp != null && sensorIdProp.PropertyType == typeof(int))
                        sensorIdProp.SetValue(mesure, sensor.LocalId);
                    var sensorDeviceIdProp = typeof(EcoAssistant.Domain.Entities.Mesure).GetProperty("SensorDeviceId");
                    if (sensorDeviceIdProp != null && sensorDeviceIdProp.PropertyType == typeof(int))
                        sensorDeviceIdProp.SetValue(mesure, sensor.DeviceId);

                    db.Set<EcoAssistant.Domain.Entities.Mesure>().Add(mesure);
                    await db.SaveChangesAsync(stoppingToken);

                    _logger.LogInformation("Saved measure for sensor db id {SensorDbId} value={Value}", sensor.PublicId,
                        msg.Value);
                }
                else
                {
                    _logger.LogDebug("Ignored MQTT topic {Topic}", topic);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing MQTT message");
            }
        };

        // Retry connect loop
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (!_client.IsConnected)
                    await _client.ConnectAsync(mqttOptions, stoppingToken);

                break; // exit loop if connected
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "MQTT connect failed; retrying in 5s");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }


    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_client != null && _client.IsConnected)
            await _client.DisconnectAsync();

        await base.StopAsync(cancellationToken);
    }

    private class MeasureMessage
    {
        public double Value { get; set; }
        public bool Alerted { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
    }
}
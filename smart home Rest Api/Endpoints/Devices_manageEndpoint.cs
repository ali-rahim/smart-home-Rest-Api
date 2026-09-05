using AutoMapper;
using Entity_class.Domain.Devices.Base;
using Microsoft.AspNetCore.OutputCaching;
using services;
using smart_home_Asp.net.Domain.Devices.Base;
using smart_home_Asp.net.Dtos;
using SmartHoe_dbcontex;

namespace smart_home_Rest_Api.Endpoints
{
    public static  class Devices_manageEndpoint
    {
        static string cachkey = "mdevices";

        public static WebApplication mapDevices_manage(this WebApplication app, string prefix)
        {

            var devices = app.MapGroup("/devices");

            devices.MapPost("/{deviceId:int}/turn-on",
                TurnOn);

            devices.MapPost("/{deviceId:int}/turn-off",
                TurnOff);

            devices.MapPost("/{deviceId:int}/value",
               RecordSensorValue);

            devices.MapGet("/{deviceId:int}/value",
                GetLatestSensorValue);

            devices.MapPost("/{deviceId:int}/status",
                RecordDigitalStatus);

            devices.MapGet("/{deviceId:int}/status",
                GetLatestDigitalStatus);

            devices.MapGet("/{deviceId:int}/readings",
                GetHistory);

            return app;

        }

        public static async Task<IResult> TurnOn(
            int deviceId,
            DeviceCommandManager commandManager)
        {
            var device = await commandManager.TurnOnAsync(deviceId);

            return device is null
                ? Results.NotFound($"Device {deviceId} not found.")
                : Results.NoContent();
        }
            

            public static async Task<IResult> TurnOff(
                int deviceId,
                DeviceCommandManager commandManager)
            {
                var device = await commandManager.TurnOffAsync(deviceId);

                return device is null
                    ? Results.NotFound($"Device {deviceId} not found.")
                    : Results.NoContent();
            }
        

    
            public static async Task<IResult> RecordSensorValue(
                int deviceId,
                RecordSensorValueRequest request,
                DeviceReadingManager readingManager)
            {
                var reading =
                    await readingManager.RecordSensorValueAsync(
                        deviceId,
                        request.Value);

                return reading is null
                    ? Results.NotFound($"Device {deviceId} not found.")
                    : Results.Ok(ToReadingResponse(reading));
            }

            public static async Task<IResult> GetLatestSensorValue(
                int deviceId,
                DeviceReadingManager readingManager)
            {
                var reading =
                    await readingManager.GetLatestAsync(
                        deviceId,
                        ReadingKind.SensorValue);

                return reading is null
                    ? Results.NotFound(
                        $"No sensor readings for device {deviceId}.")
                    : Results.Ok(ToReadingResponse(reading));
            }

            public static async Task<IResult> RecordDigitalStatus(
                int deviceId,
                RecordDigitalStatusRequest request,
                DeviceReadingManager readingManager)
            {
                var reading =
                    await readingManager.RecordDigitalStatusAsync(
                        deviceId,
                        request.Status);

                return reading is null
                    ? Results.NotFound($"Device {deviceId} not found.")
                    : Results.Ok(ToReadingResponse(reading));
            }

            public static async Task<IResult> GetLatestDigitalStatus(
                int deviceId,
                DeviceReadingManager readingManager)
            {
                var reading =
                    await readingManager.GetLatestAsync(
                        deviceId,
                        ReadingKind.DigitalStatus);

                return reading is null
                    ? Results.NotFound(
                        $"No status readings for device {deviceId}.")
                    : Results.Ok(ToReadingResponse(reading));
            }

            public static async Task<IResult> GetHistory(
                int deviceId,
                DeviceReadingManager readingManager,
                int count = 20)
            {
                var readings =
                    await readingManager.GetHistoryAsync(
                        deviceId,
                        count);

                return Results.Ok(
                    readings.Select(ToReadingResponse));
            }

            private static DeviceReadingResponse ToReadingResponse(
                DeviceReading r)
            {
                return new DeviceReadingResponse
                {
                    Id = r.Id,
                    DeviceId = r.DeviceId,
                    Kind = r.Kind.ToString(),
                    NumericValue = r.NumericValue,
                    BoolValue = r.BoolValue,
                    RecordedAt = r.RecordedAt
                };
            }
        }
    }


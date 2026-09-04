using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using services;
using smart_home_Asp.net.Domain.Devices.Base;
using smart_home_Asp.net.Dtos;
using SmartHoe_dbcontex;

namespace smart_home_Rest_Api.Endpoints
{
    public static  class DeviceEndpoints
    {
        static string cachkey = "devices";

        public static WebApplication mapDevices(this WebApplication app, string prefix)
        {

            var devices = app.MapGroup(prefix);

            devices.MapPost("/", CreateDevice);

            devices.MapGet("/", GetDevicesByRoom)
                .CacheOutput(c => c
                    .Expire(TimeSpan.FromSeconds(15))
                    .Tag("devices"));

            devices.MapPut("/{deviceId:int}", UpdateDevice);

            devices.MapDelete("/{deviceId:int}", DeleteDevice);

            return app;

        }
        static async Task<IResult> CreateDevice(
            int roomId,
            CreateDeviceRequest request,
            DeviceManager deviceManager,
            IMapper mapper,
            IOutputCacheStore cacheStore)
        {
            if (!Enum.TryParse<DeviceType>(
                    request.DeviceType,
                    true,
                    out var type))
            {
                return Results.BadRequest(
                    $"Unknown device type '{request.DeviceType}'.");
            }

            var device = await deviceManager.CreateDeviceAsync(
                roomId,
                type,
                request.Name,
                request.ExternalId);

            if (device is null)
                return Results.NotFound(
                    $"Room {roomId} not found.");

            await cacheStore.EvictByTagAsync(cachkey, default);

            return Results.Created(
                $"/rooms/{roomId}/devices/{device.Id}",
                mapper.Map<DeviceResponse>(device));
        }


        static async Task<IResult> GetDevicesByRoom(
            int roomId,
            DeviceManager deviceManager,
            IMapper mapper)
        {
            var devices =
                await deviceManager.GetDevicesByRoomAsync(roomId);

            return Results.Ok(
                mapper.Map<List<DeviceResponse>>(devices));
        }


        static async Task<IResult> UpdateDevice(
            int roomId,
            int deviceId,
            UpdateDeviceRequest request,
            DeviceManager deviceManager,
            IMapper mapper,
            IOutputCacheStore cacheStore)
        {
            var device = await deviceManager.UpdateDeviceAsync(
                roomId,
                deviceId,
                request.Name,
                request.ExternalId);

            if (device is null)
                return Results.NotFound(
                    $"Device {deviceId} not found in room {roomId}.");

            await cacheStore.EvictByTagAsync(cachkey, default);

            return Results.Ok(
                mapper.Map<DeviceResponse>(device));
        }


        static async Task<IResult> DeleteDevice(
            int roomId,
            int deviceId,
            DeviceManager deviceManager,
            IOutputCacheStore cacheStore)
        {
            var deleted = await deviceManager.DeleteDeviceAsync(
                roomId,
                deviceId);

            if (!deleted)
                return Results.NotFound(
                    $"Device {deviceId} not found in room {roomId}.");

            await cacheStore.EvictByTagAsync(cachkey, default);

            return Results.NoContent();
        }
    }
}

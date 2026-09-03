using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;
using services;
using smart_home_Asp.net;
using smart_home_Asp.net.Configuration;
using smart_home_Asp.net.Domain.Devices.ability_interfaces;
using smart_home_Asp.net.Domain.Devices.Base;
using smart_home_Asp.net.Domain.Entities;
using smart_home_Asp.net.YourProjectName.Middleware;
using SmartHoe_dbcontex;
using System.Threading.RateLimiting;
using static Azure.Core.HttpHeader;
using smart_home_Asp.net.Dtos;
public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //Serilog
        builder.Host.UseSerilog((context, services, configuration) =>
        {
            configuration
                .MinimumLevel.Information() // سطح کلی: Information
                .MinimumLevel.Override("smart_home_Asp.net", LogEventLevel.Debug) // Override برای پروژه
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate:
                    "serilag")
                .ReadFrom.Configuration(context.Configuration); //seq هم تنظیم شده
        });

        //RateLimiter
        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.GlobalLimiter =
                PartitionedRateLimiter.Create<HttpContext, string>(
                    httpContext =>
                        RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey:
                                httpContext.Connection.RemoteIpAddress?.ToString()
                                ?? "unknown",

                            factory: _ => new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = 100,
                                Window = TimeSpan.FromMinutes(1),
                                QueueLimit = 0
                            }));
        });



        //AddHsts
        builder.Services.AddHsts(opts => {
            opts.MaxAge = TimeSpan.FromDays(1);
            opts.IncludeSubDomains = true;
        });


        //Cache
        builder.Services.AddOutputCache();


        //DI
        builder.Services.AddScoped<HomeManager>();

        builder.Services.AddScoped<DeviceManager>();
        builder.Services.AddScoped<RoomManager>();



        //builder.Services.AddScoped<HomeService>();




        builder.Services.AddOpenApi();
        builder.Services.AddDbContext<SmartHome_dbcontex>((serviceProvider, options) =>
        {
            var storageOptions = serviceProvider.GetRequiredService<IOptions<StorageOptions>>();
            options.UseSqlServer(storageOptions.Value.ConnectionStrings);
        });






        //Configuration
        builder.Services.Configure<SmartHomeOptions>(
        builder.Configuration.GetSection("SmartHome"));
        builder.Services.Configure<StorageOptions>(
        builder.Configuration.GetSection("Storage"));



        var app = builder.Build();




        //Middleware
        app.UseHsts();
        app.UseHttpsRedirection();
        app.UseExceptionHandlingMiddleware();
        app.UseRequestLoggingMiddleware();
        app.UseRateLimiter();
        app.MapOpenApi();
        app.MapScalarApiReference();
        app.UseOutputCache();





        //تست endpoint
        //app.Run(async c =>
        //{
        //    var endpoint = c.GetEndpoint();
        //    if (endpoint is not null)
        //        await c.Response.WriteAsync(endpoint.DisplayName ?? "NoName");
        //});

        //app.MapFallback(async (httpContext) =>
        //{
        //    await httpContext.Response.WriteAsync("Not Found");
        //});




        // ---------- End points ----------



        app.MapGet("/", () => "Hello World!");
        // ---------- home ----------

        app.MapPost("/homes", async (Home home, HomeManager homeManager) =>
        {

            var result = await homeManager.InsertdbhomeAsync(home);
            return Results.Created($"/homes/{result}", home);

        }).CacheOutput(c =>
        {
            c.Expire(TimeSpan.FromSeconds(15)).Tag("homes");
        });

        app.MapGet("/homes" , async (HomeManager homeManager ,IOutputCacheStore CacheStore ) =>
        {
            var result = await homeManager.get_homeAsync();
            await CacheStore.EvictByTagAsync("homes", default);
            return Results.Ok<List<Home>>(result);


        });

        app.MapGet("/homes/{id:int}", async (HomeManager homeManager , int id ) =>
        {
            var result = await homeManager.get_homeAsync(id);
            if(result == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(result);

        });

        // ---------- Room ----------

        app.MapPost("/homes/{homeId:int}/rooms", async (int homeId, CreateRoomRequest request, RoomManager roomManager) =>
        {
            var room = await roomManager.CreateRoomAsync(homeId, request.Name);
            if (room is null)
                return Results.NotFound($"Home {homeId} not found.");

            return Results.Created($"/homes/{homeId}/rooms/{room.Id}", room);
        });

        app.MapGet("/homes/{homeId:int}/rooms", async (int homeId, RoomManager roomManager) =>
        {
            var rooms = await roomManager.GetRoomsByHomeAsync(homeId);
            return Results.Ok(rooms);
        });

        // ---------- Device ----------

        app.MapPost("/rooms/{roomId:int}/devices", async (int roomId, CreateDeviceRequest request, DeviceManager deviceManager) =>
        {
            if (!Enum.TryParse<DeviceType>(request.DeviceType, true, out var type))
                return Results.BadRequest($"Unknown device type '{request.DeviceType}'.");

            var device = await deviceManager.CreateDeviceAsync(roomId, type, request.Name, request.ExternalId);
            if (device is null)
                return Results.NotFound($"Room {roomId} not found.");

            return Results.Created($"/rooms/{roomId}/devices/{device.Id}", device);
        });

        app.MapGet("/rooms/{roomId:int}/devices", async (int roomId, DeviceManager deviceManager) =>
        {
            var devices = await deviceManager.GetDevicesByRoomAsync(roomId);
            return Results.Ok(devices);
        });


        //// ---------- Room ----------
        //var rooms = app.MapGroup("/rooms");


        //app.MapGet("/room/{id}", (string id, HomeService homeService) =>
        //{

        //        var room = homeService.GetRoomById(id);
        //        return Results.Ok(room);

        //})
        //.WithMetadata(new RouteNameMetadata("room"));

        //rooms.MapPost("/{roomId}", async (string roomId, HttpContext context, HomeService homeService) =>
        //{

        //       await homeService.AddRoom(roomId);

        //        var linkGenerator = context.RequestServices.GetService<LinkGenerator>();
        //        string path = linkGenerator.GetPathByRouteValues(context, "room", new { id = roomId });

        //        return Results.Created(path, roomId);

        //});

        //rooms.MapDelete("/{roomId}", (string roomId, HomeService homeService) =>
        //{

        //        homeService.RemoveRoom(roomId);
        //        return Results.NoContent();

        //});

        //rooms.MapGet("/", (HomeService homeService) =>
        //{
        //    var rooms = homeService.GetAllRooms();
        //    return Results.Ok(rooms);
        //});


        //// ---------- Capability mapping ----------

        //var capabilityMap = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
        //{
        //    ["switchable"] = typeof(Iswitchable),
        //    ["analog"] = typeof(Ianalog),
        //    ["digital"] = typeof(Idigital),
        //};

        //// ---------- Device ----------



        //app.MapPost("device/{id}/Turn_on_off", (string id, HomeService homeService) =>
        //{
        //    var device = homeService.GetDeviceById(id);
        //    if (device is not Iswitchable )
        //        return Results.BadRequest("This device does not support switchable capability.");
        //    homeService.Turn_on_off(device);
        //    return Results.Ok(device);
        //});


        //app.MapGet("device/{id}/sensor_value", (string id, HomeService homeService) =>
        //{
        //    var device = homeService.GetDeviceById(id);

        //      if (device is not Ianalog && device is not Idigital)
        //      return Results.BadRequest("This device does not support sensor capability.");

        //      var value = homeService.get_Status(device);
        //       return Results.Ok(value);
        //});



        //app.MapGet("device/{id}", (string id, HomeService homeService) =>
        //{

        //        var device = homeService.GetDeviceById(id);
        //        return Results.Ok(device);

        //})
        //.WithMetadata(new RouteNameMetadata("device"));





        //var devices = app.MapGroup("/devices");

        //devices.MapPost("/{type}/{deviceId}", (DeviceType type, string deviceId, HttpContext context, HomeService homeService) =>
        //{

        //        homeService.CreateDevice(type, deviceId);

        //        var linkGenerator = context.RequestServices.GetService<LinkGenerator>();
        //        string path = linkGenerator.GetPathByRouteValues(context, "device", new { id = deviceId });

        //        return Results.Created(path, deviceId);

        //});

        //devices.MapDelete("/{deviceId}", (string deviceId, HomeService homeService) =>
        //{

        //        homeService.RemoveDeviceCompletely(deviceId);
        //        return Results.NoContent();

        //});

        //// ---------- GET /devices?capability=switchable ----------

        //devices.MapGet("/", (string? capability, HomeService homeService) =>
        //{
        //    if (string.IsNullOrWhiteSpace(capability))
        //        return Results.Ok(homeService.GetAllDevices());

        //    if (!capabilityMap.TryGetValue(capability, out var type))
        //        return Results.BadRequest($"Unknown capability '{capability}'.");

        //    var method = typeof(HomeService)
        //        .GetMethod(nameof(HomeService.GetDevicesByCapability))!
        //        .MakeGenericMethod(type);

        //    var result = method.Invoke(homeService, null);
        //    return Results.Ok(result);
        //});

        //// ---------- Room and Device ----------


        //rooms.MapPost("/{roomId}/devices/{deviceId}", (string roomId, string deviceId, HomeService homeService) =>
        //{

        //        homeService.AddDeviceToRoom(deviceId, roomId);
        //        return Results.Ok();

        //});

        //rooms.MapDelete("/{roomId}/devices/{deviceId}", (string roomId, string deviceId, HomeService homeService) =>
        //{

        //        homeService.RemoveDeviceFromRoom(roomId, deviceId);
        //        return Results.NoContent();

        //});



        //// ---------- GET /rooms/{roomId}/devices?capability=analog ----------

        //rooms.MapGet("/{roomId}/devices", (string roomId, string? capability, HomeService homeService) =>
        //{

        //        Type type = typeof(Entity); // پیش‌فرض: همه‌ی دستگاه‌ها

        //        if (!string.IsNullOrWhiteSpace(capability))
        //        {
        //            if (!capabilityMap.TryGetValue(capability, out type!))
        //                return Results.BadRequest($"Unknown capability '{capability}'.");
        //        }

        //        var method = typeof(HomeService)
        //            .GetMethod(nameof(HomeService.GetDevicesInRoomByCapability))!
        //            .MakeGenericMethod(type);

        //        var result = method.Invoke(homeService, new object[] { roomId });
        //        return Results.Ok(result);

        //});

        //// ---------- POST /rooms/{roomId}/devices/{type}/{deviceId} ----------

        //rooms.MapPost("/{roomId}/devices/{type}/{deviceId}",
        //    (string roomId, DeviceType type, string deviceId, HttpContext context, HomeService homeService) =>
        //    {

        //            homeService.CreateDeviceInRoom(type, deviceId, roomId);

        //            var linkGenerator = context.RequestServices.GetService<LinkGenerator>();
        //            string path = linkGenerator.GetPathByRouteValues(context, "device", new { id = deviceId });

        //            return Results.Created(path, deviceId);

        //    });



        ////----------get Configuration----------




        //app.MapGet("/config", (
        //           IOptions<SmartHomeOptions> smartHomeOptions,
        //           IOptions<StorageOptions> storageOptions) =>
        //        {
        //            return Results.Ok(new
        //            {
        //                SmartHome = smartHomeOptions.Value,
        //                Storage = storageOptions.Value
        //            });
        //        });


        app.Run();

    }
}
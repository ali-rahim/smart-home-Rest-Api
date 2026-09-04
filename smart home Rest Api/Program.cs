
using smart_home_Rest_Api.serviceandpipeline;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.ConfigureService();
        app.ConfigurePipeline();
        app.Run();

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





    }
}
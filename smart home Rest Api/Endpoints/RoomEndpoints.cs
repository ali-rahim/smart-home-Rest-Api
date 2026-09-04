using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using services;
using smart_home_Asp.net.Dtos;
using SmartHoe_dbcontex;

namespace smart_home_Rest_Api.Endpoints
{
    public static class RoomEndpoints
    {
        static string cachkey = "rooms";

        public static WebApplication mapRooms(this WebApplication app, string prefix)
        {
            var rooms = app.MapGroup(prefix);

            rooms.MapPost("/", CreateRoom);

            rooms.MapGet("/", GetRoomsByHome)
                .CacheOutput(c => c
                    .Expire(TimeSpan.FromSeconds(15))
                    .Tag("rooms"));

            rooms.MapPut("/{roomId:int}", UpdateRoom);

            rooms.MapDelete("/{roomId:int}", DeleteRoom);
            return app;

        }

        static async Task<IResult> CreateRoom(
            int homeId,
            RoomRequest request,
            RoomManager roomManager,
            IMapper mapper,
            IOutputCacheStore cacheStore)
        {
            var room = await roomManager.CreateRoomAsync(homeId, request.Name);

            if (room is null)
                return Results.NotFound($"Home {homeId} not found.");

            await cacheStore.EvictByTagAsync(cachkey, default);

            return Results.Created(
                $"/homes/{homeId}/rooms/{room.Id}",
                mapper.Map<RoomResponse>(room));
        }


        static async Task<IResult> GetRoomsByHome(
            int homeId,
            RoomManager roomManager,
            IMapper mapper)
        {
            var rooms = await roomManager.GetRoomsByHomeAsync(homeId);

            return Results.Ok(
                mapper.Map<List<RoomResponse>>(rooms));
        }


        static async Task<IResult> UpdateRoom(
            int homeId,
            int roomId,
            RoomRequest request,
            RoomManager roomManager,
            IMapper mapper,
            IOutputCacheStore cacheStore)
        {
            var room = await roomManager.UpdateRoomAsync(
                homeId,
                roomId,
                request.Name);

            if (room is null)
                return Results.NotFound(
                    $"Room {roomId} not found in home {homeId}.");

            await cacheStore.EvictByTagAsync(cachkey, default);

            return Results.Ok(
                mapper.Map<RoomResponse>(room));
        }


        static async Task<IResult> DeleteRoom(
            int homeId,
            int roomId,
            RoomManager roomManager,
            IOutputCacheStore cacheStore)
        {
            var deleted = await roomManager.DeleteRoomAsync(
                homeId,
                roomId);

            if (!deleted)
                return Results.NotFound(
                    $"Room {roomId} not found in home {homeId}.");

            await cacheStore.EvictByTagAsync(cachkey, default);

            return Results.NoContent();
        }

    }
}

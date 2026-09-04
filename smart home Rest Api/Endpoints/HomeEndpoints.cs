using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Hosting.Internal;
using services;
using smart_home_Asp.net.Domain.Entities;
using smart_home_Asp.net.Dtos;
using SmartHoe_dbcontex;

namespace smart_home_Rest_Api.Endpoints
{
    public static class HomeEndpoints
    {
        static string cachkey = "homes";
        public static WebApplication mapHomes(this WebApplication app ,string prefix)
        {
            var homes = app.MapGroup(prefix);

            homes.MapPost("/", CreateHome);
            homes.MapGet("/", GetHomes)
                .CacheOutput(c => c
                    .Expire(TimeSpan.FromSeconds(15))
                    .Tag("homes"));
            homes.MapGet("/{id:int}", GetHomeById)
                .CacheOutput(c => c
                    .Expire(TimeSpan.FromSeconds(15))
                    .Tag("homes"));
            homes.MapPut("/{id:int}", UpdateHome);
            homes.MapDelete("/{id:int}", DeleteHome);
            return app;

        }
        static async Task<IResult> CreateHome(
           CreateHomeRequest request,
           HomeManager homeManager,
           IOutputCacheStore cacheStore,
           IMapper mapper)
        {
            var home = new Home(request.Name);

            var id = await homeManager.InsertdbhomeAsync(home);

            await cacheStore.EvictByTagAsync(cachkey, default);

            var created = await homeManager.get_homeAsync(id);

            return Results.Created(
                $"/homes/{id}",
                mapper.Map<HomeResponse>(created));
        }


        static async Task<IResult> GetHomes(
            HomeManager homeManager,
            IMapper mapper)
        {
            var homes = await homeManager.get_homeAsync();

            return Results.Ok(
                mapper.Map<List<HomeResponse>>(homes));
        }


        static async Task<IResult> GetHomeById(
            HomeManager homeManager,
            int id,
            IMapper mapper)
        {
            var home = await homeManager.get_homeAsync(id);

            return home is null
                ? Results.NotFound()
                : Results.Ok(mapper.Map<HomeResponse>(home));
        }


        static async Task<IResult> UpdateHome(
            int id,
            UpdateHomeRequest request,
            HomeManager homeManager,
            IMapper mapper,
            IOutputCacheStore cacheStore)
        {
            var home = await homeManager.UpdateHomeAsync(id, request.Name);

            if (home is null)
                return Results.NotFound($"Home {id} not found.");

            await cacheStore.EvictByTagAsync(cachkey, default);

            return Results.Ok(
                mapper.Map<HomeResponse>(home));
        }


        static async Task<IResult> DeleteHome(
            int id,
            HomeManager homeManager,
            IOutputCacheStore cacheStore)
        {
            var deleted = await homeManager.DeleteHomeAsync(id);

            if (!deleted)
                return Results.NotFound($"Home {id} not found.");

            await cacheStore.EvictByTagAsync(cachkey, default);

            return Results.NoContent();
        }

    }
}

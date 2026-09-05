using DeviceCommunicator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;
using services;
using smart_home_Asp.net;
using smart_home_Asp.net.Configuration;
using smart_home_Asp.net.Mapping;
using smart_home_Asp.net.YourProjectName.Middleware;
using smart_home_Rest_Api.Endpoints;
using SmartHoe_dbcontex;
using System.Threading.RateLimiting;

namespace smart_home_Rest_Api.serviceandpipeline
{
    public static class HostingExtensions
    {
        public static WebApplication ConfigureService(this WebApplicationBuilder builder)
        {
            //AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            //Serilog
            builder.Host.UseSerilog((context, services, configuration) =>
            {
                configuration
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("smart_home_Asp.net", LogEventLevel.Debug)
                    .Enrich.FromLogContext()
                    .WriteTo.Console(outputTemplate: "serilag")
                    .ReadFrom.Configuration(context.Configuration);
            });

            //RateLimiter
            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.GlobalLimiter =
                    PartitionedRateLimiter.Create<HttpContext, string>(
                        partitionKey:
                            httpContext.Connection.RemoteIpAddress?.ToString()
                            ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 100,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        });
            });

            //OpenApi
            builder.Services.AddOpenApi();

            //AddHsts
            builder.Services.AddHsts(opts =>
            {
                opts.MaxAge = TimeSpan.FromDays(1);
                opts.IncludeSubDomains = true;
            });

            //Cache
            builder.Services.AddOutputCache();

            //DI
            builder.Services.AddScoped<HomeManager>();
            builder.Services.AddScoped<DeviceManager>();
            builder.Services.AddScoped<RoomManager>();
            builder.Services.AddScoped<DeviceCommandManager>();
            builder.Services.AddScoped<DeviceReadingManager>();

            builder.Services.AddSingleton<MqttDeviceCommunicator>();
            builder.Services.AddSingleton<IDeviceCommunicator>(
                serviceProvider => serviceProvider.GetRequiredService<MqttDeviceCommunicator>());
            builder.Services.AddHostedService(
                serviceProvider => serviceProvider.GetRequiredService<MqttDeviceCommunicator>());

            //EF
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

            return builder.Build();
        }

        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            //Middleware
            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseExceptionHandlingMiddleware();
            app.UseRequestLoggingMiddleware();
            app.UseRateLimiter();
            app.MapOpenApi();
            app.MapScalarApiReference();
            app.UseOutputCache();

            //Endpoints
            app.mapHomes("/homes");
            app.mapRooms("/homes/{homeId:int}/rooms");
            app.mapDevices("/rooms/{roomId:int}/devices");
            app.mapDevices_manage("/devices");

            return app;
        }
    }
}

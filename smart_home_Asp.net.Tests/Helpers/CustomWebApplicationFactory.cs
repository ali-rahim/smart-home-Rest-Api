using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using smart_home_Asp.net.Domain.Entities;
using smart_home_Asp.net.Services;

namespace smart_home_Asp.net.Tests.Helpers;

/// <summary>
/// WebApplicationFactory سفارشی برای Integration Test.
/// هر تست یک Home تازه و ایزوله می‌گیرد.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            // حذف ثبت‌های قبلی Home / Managerها تا هر تست ایزوله باشد
            services.RemoveAll<Home>();
            services.RemoveAll<DeviceManager>();
            services.RemoveAll<RoomManager>();
            services.RemoveAll<HomeService>();

            services.AddSingleton(new Home("Test Home"));
            services.AddSingleton<DeviceManager>();
            services.AddSingleton<RoomManager>();
            services.AddSingleton<HomeService>();
        });
    }
}

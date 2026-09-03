using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using smart_home_Asp.net.Tests.Helpers;
using Xunit;

namespace smart_home_Asp.net.Tests.Integration;

/// <summary>
/// سناریوی end-to-end کامل: ساخت خانه → اتاق → دستگاه → کنترل → سنسور → پاکسازی
/// </summary>
public class FullWorkflowTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public FullWorkflowTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Complete_SmartHome_Workflow()
    {
        var roomId = $"wf-room-{Guid.NewGuid():N}".Substring(0, 16);
        var lightId = $"wf-light-{Guid.NewGuid():N}".Substring(0, 16);
        var fanId = $"wf-fan-{Guid.NewGuid():N}".Substring(0, 16);
        var doorId = $"wf-door-{Guid.NewGuid():N}".Substring(0, 16);
        var rainId = $"wf-rain-{Guid.NewGuid():N}".Substring(0, 16);

        // 1) ایجاد اتاق
        var createRoom = await _client.PostAsync($"/rooms/{roomId}", null);
        createRoom.StatusCode.Should().Be(HttpStatusCode.Created);

        // 2) ایجاد دستگاه‌ها داخل اتاق
        (await _client.PostAsync($"/rooms/{roomId}/devices/Light/{lightId}", null))
            .StatusCode.Should().Be(HttpStatusCode.Created);
        (await _client.PostAsync($"/rooms/{roomId}/devices/fan/{fanId}", null))
            .StatusCode.Should().Be(HttpStatusCode.Created);
        (await _client.PostAsync($"/rooms/{roomId}/devices/door_sensor/{doorId}", null))
            .StatusCode.Should().Be(HttpStatusCode.Created);
        (await _client.PostAsync($"/rooms/{roomId}/devices/rain_sensor/{rainId}", null))
            .StatusCode.Should().Be(HttpStatusCode.Created);

        // 3) لیست دستگاه‌های اتاق
        var devicesInRoom = await _client.GetFromJsonAsync<List<JsonElement>>(
            $"/rooms/{roomId}/devices", JsonOpts);
        devicesInRoom!.Should().HaveCount(4);

        // 4) فیلتر capability
        var switchables = await _client.GetFromJsonAsync<List<JsonElement>>(
            $"/rooms/{roomId}/devices?capability=switchable", JsonOpts);
        switchables!.Should().HaveCount(2); // light + fan

        var analogs = await _client.GetFromJsonAsync<List<JsonElement>>(
            $"/rooms/{roomId}/devices?capability=analog", JsonOpts);
        analogs!.Should().ContainSingle();

        var digitals = await _client.GetFromJsonAsync<List<JsonElement>>(
            $"/rooms/{roomId}/devices?capability=digital", JsonOpts);
        digitals!.Should().ContainSingle();

        // 5) روشن کردن چراغ
        var toggle = await _client.PostAsync($"/device/{lightId}/Turn_on_off", null);
        toggle.StatusCode.Should().Be(HttpStatusCode.OK);
        var toggled = await toggle.Content.ReadFromJsonAsync<JsonElement>(JsonOpts);
        toggled.GetProperty("isOn").GetBoolean().Should().BeTrue();

        // 6) خواندن سنسورها
        var doorVal = await _client.GetFromJsonAsync<bool>($"/device/{doorId}/sensor_value");
        doorVal.Should().BeTrue();

        var rainVal = await _client.GetFromJsonAsync<double>($"/device/{rainId}/sensor_value");
        rainVal.Should().Be(25.85);

        // 7) جدا کردن دستگاه از اتاق (خود دستگاه باقی می‌ماند)
        (await _client.DeleteAsync($"/rooms/{roomId}/devices/{fanId}"))
            .StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await _client.GetAsync($"/device/{fanId}"))
            .StatusCode.Should().Be(HttpStatusCode.OK);

        // 8) حذف کامل دستگاه
        (await _client.DeleteAsync($"/devices/{lightId}"))
            .StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await _client.GetAsync($"/device/{lightId}"))
            .StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.InternalServerError);

        // 9) حذف اتاق
        (await _client.DeleteAsync($"/rooms/{roomId}"))
            .StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}

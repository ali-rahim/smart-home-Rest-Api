using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using smart_home_Asp.net.Tests.Helpers;
using Xunit;

namespace smart_home_Asp.net.Tests.Integration;

public class DevicesApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public DevicesApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    // ---------- POST /devices/{type}/{deviceId} ----------

    [Theory]
    [InlineData("Light")]
    [InlineData("fan")]
    [InlineData("dozdgir")]
    [InlineData("door_sensor")]
    [InlineData("rain_sensor")]
    public async Task CreateDevice_All_Types_Should_Return_201(string type)
    {
        var id = $"dev-{type}-{Guid.NewGuid():N}".Substring(0, 20);
        var response = await _client.PostAsync($"/devices/{type}/{id}", null);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateDevice_Duplicate_Should_Fail()
    {
        var id = $"dup-{Guid.NewGuid():N}".Substring(0, 16);
        await _client.PostAsync($"/devices/Light/{id}", null);
        var response = await _client.PostAsync($"/devices/Light/{id}", null);

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.Conflict,
            HttpStatusCode.InternalServerError);
    }

    // ---------- GET /device/{id} ----------

    [Fact]
    public async Task GetDeviceById_Should_Return_Device()
    {
        var id = $"get-{Guid.NewGuid():N}".Substring(0, 16);
        await _client.PostAsync($"/devices/Light/{id}", null);

        var response = await _client.GetAsync($"/device/{id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOpts);
        json.GetProperty("id").GetString().Should().Be(id);
    }

    [Fact]
    public async Task GetDeviceById_Missing_Should_Fail()
    {
        var response = await _client.GetAsync("/device/no-such-device");

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NotFound,
            HttpStatusCode.InternalServerError);
    }

    // ---------- DELETE /devices/{deviceId} ----------

    [Fact]
    public async Task DeleteDevice_Should_Return_204()
    {
        var id = $"del-{Guid.NewGuid():N}".Substring(0, 16);
        await _client.PostAsync($"/devices/fan/{id}", null);

        var response = await _client.DeleteAsync($"/devices/{id}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var get = await _client.GetAsync($"/device/{id}");
        get.StatusCode.Should().BeOneOf(
            HttpStatusCode.NotFound,
            HttpStatusCode.InternalServerError);
    }

    // ---------- GET /devices ----------

    [Fact]
    public async Task GetAllDevices_Should_Return_List()
    {
        await _client.PostAsync($"/devices/Light/all-l-{Guid.NewGuid():N}".Substring(0, 20), null);
        await _client.PostAsync($"/devices/fan/all-f-{Guid.NewGuid():N}".Substring(0, 20), null);

        var response = await _client.GetAsync("/devices");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var devices = await response.Content.ReadFromJsonAsync<List<JsonElement>>(JsonOpts);
        devices.Should().NotBeNull();
        devices!.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    // ---------- GET /devices?capability= ----------

    [Theory]
    [InlineData("switchable")]
    [InlineData("analog")]
    [InlineData("digital")]
    public async Task GetDevices_By_Capability_Should_Return_200(string capability)
    {
        // آماده‌سازی داده مرتبط
        await _client.PostAsync($"/devices/Light/sw-{Guid.NewGuid():N}".Substring(0, 18), null);
        await _client.PostAsync($"/devices/rain_sensor/an-{Guid.NewGuid():N}".Substring(0, 18), null);
        await _client.PostAsync($"/devices/door_sensor/dg-{Guid.NewGuid():N}".Substring(0, 18), null);

        var response = await _client.GetAsync($"/devices?capability={capability}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetDevices_Unknown_Capability_Should_Return_400()
    {
        var response = await _client.GetAsync("/devices?capability=flying");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ---------- POST device/{id}/Turn_on_off ----------

    [Fact]
    public async Task TurnOnOff_Switchable_Should_Toggle()
    {
        var id = $"tog-{Guid.NewGuid():N}".Substring(0, 16);
        await _client.PostAsync($"/devices/Light/{id}", null);

        var before = await _client.GetAsync($"/device/{id}");
        var beforeJson = await before.Content.ReadFromJsonAsync<JsonElement>(JsonOpts);
        var wasOn = beforeJson.TryGetProperty("isOn", out var p) && p.GetBoolean();

        var response = await _client.PostAsync($"/device/{id}/Turn_on_off", null);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var afterJson = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOpts);
        afterJson.GetProperty("isOn").GetBoolean().Should().Be(!wasOn);
    }

    [Fact]
    public async Task TurnOnOff_NonSwitchable_Should_Return_400()
    {
        var id = $"ns-{Guid.NewGuid():N}".Substring(0, 16);
        await _client.PostAsync($"/devices/rain_sensor/{id}", null);

        var response = await _client.PostAsync($"/device/{id}/Turn_on_off", null);
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ---------- GET device/{id}/sensor_value ----------

    [Fact]
    public async Task SensorValue_Digital_Should_Return_Value()
    {
        var id = $"ds-{Guid.NewGuid():N}".Substring(0, 16);
        await _client.PostAsync($"/devices/door_sensor/{id}", null);

        var response = await _client.GetAsync($"/device/{id}/sensor_value");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var value = await response.Content.ReadFromJsonAsync<bool>();
        // مقدار پیش‌فرض door_sensor = true
        value.Should().BeTrue();
    }

    [Fact]
    public async Task SensorValue_Analog_Should_Return_Value()
    {
        var id = $"rs-{Guid.NewGuid():N}".Substring(0, 16);
        await _client.PostAsync($"/devices/rain_sensor/{id}", null);

        var response = await _client.GetAsync($"/device/{id}/sensor_value");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var value = await response.Content.ReadFromJsonAsync<double>();
        value.Should().Be(25.85);
    }

    [Fact]
    public async Task SensorValue_NonSensor_Should_Return_400()
    {
        var id = $"ns2-{Guid.NewGuid():N}".Substring(0, 16);
        await _client.PostAsync($"/devices/Light/{id}", null);

        var response = await _client.GetAsync($"/device/{id}/sensor_value");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

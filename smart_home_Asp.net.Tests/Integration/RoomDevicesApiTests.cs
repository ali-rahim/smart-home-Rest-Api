using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using smart_home_Asp.net.Tests.Helpers;
using Xunit;

namespace smart_home_Asp.net.Tests.Integration;

public class RoomDevicesApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public RoomDevicesApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task EnsureRoomAsync(string roomId)
    {
        await _client.PostAsync($"/rooms/{roomId}", null);
    }

    // ---------- POST /rooms/{roomId}/devices/{type}/{deviceId} ----------

    [Fact]
    public async Task CreateDeviceInRoom_Should_Return_201()
    {
        var roomId = $"rm-{Guid.NewGuid():N}".Substring(0, 12);
        var deviceId = $"dv-{Guid.NewGuid():N}".Substring(0, 12);
        await EnsureRoomAsync(roomId);

        var response = await _client.PostAsync($"/rooms/{roomId}/devices/Light/{deviceId}", null);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        // دستگاه باید در اتاق باشد
        var list = await _client.GetAsync($"/rooms/{roomId}/devices");
        list.StatusCode.Should().Be(HttpStatusCode.OK);
        var devices = await list.Content.ReadFromJsonAsync<List<JsonElement>>(JsonOpts);
        devices!.Any(d => d.GetProperty("id").GetString() == deviceId).Should().BeTrue();
    }

    [Fact]
    public async Task CreateDeviceInRoom_When_Room_Missing_Should_Fail_And_Not_Leave_Orphan()
    {
        var deviceId = $"orphan-{Guid.NewGuid():N}".Substring(0, 16);

        var response = await _client.PostAsync($"/rooms/no-room/devices/Light/{deviceId}", null);

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NotFound,
            HttpStatusCode.InternalServerError);

        // rollback: دستگاه نباید در سیستم باقی مانده باشد
        var get = await _client.GetAsync($"/device/{deviceId}");
        get.StatusCode.Should().BeOneOf(
            HttpStatusCode.NotFound,
            HttpStatusCode.InternalServerError);
    }

    // ---------- POST /rooms/{roomId}/devices/{deviceId} (attach existing) ----------

    [Fact]
    public async Task AttachExistingDevice_To_Room_Should_Return_200()
    {
        var roomId = $"rm-{Guid.NewGuid():N}".Substring(0, 12);
        var deviceId = $"dv-{Guid.NewGuid():N}".Substring(0, 12);
        await EnsureRoomAsync(roomId);
        await _client.PostAsync($"/devices/fan/{deviceId}", null);

        var response = await _client.PostAsync($"/rooms/{roomId}/devices/{deviceId}", null);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ---------- DELETE /rooms/{roomId}/devices/{deviceId} ----------

    [Fact]
    public async Task RemoveDeviceFromRoom_Should_Return_204_But_Device_Still_Exists()
    {
        var roomId = $"rm-{Guid.NewGuid():N}".Substring(0, 12);
        var deviceId = $"dv-{Guid.NewGuid():N}".Substring(0, 12);
        await EnsureRoomAsync(roomId);
        await _client.PostAsync($"/rooms/{roomId}/devices/Light/{deviceId}", null);

        var response = await _client.DeleteAsync($"/rooms/{roomId}/devices/{deviceId}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // دستگاه هنوز در DeviceManager هست
        var get = await _client.GetAsync($"/device/{deviceId}");
        get.StatusCode.Should().Be(HttpStatusCode.OK);

        // اما در اتاق نیست
        var list = await _client.GetAsync($"/rooms/{roomId}/devices");
        var devices = await list.Content.ReadFromJsonAsync<List<JsonElement>>(JsonOpts);
        devices!.Any(d => d.GetProperty("id").GetString() == deviceId).Should().BeFalse();
    }

    // ---------- GET /rooms/{roomId}/devices?capability= ----------

    [Fact]
    public async Task GetDevicesInRoom_All_Should_Return_Devices()
    {
        var roomId = $"rm-{Guid.NewGuid():N}".Substring(0, 12);
        await EnsureRoomAsync(roomId);
        await _client.PostAsync($"/rooms/{roomId}/devices/Light/l-in-room", null);
        await _client.PostAsync($"/rooms/{roomId}/devices/rain_sensor/r-in-room", null);

        var response = await _client.GetAsync($"/rooms/{roomId}/devices");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var devices = await response.Content.ReadFromJsonAsync<List<JsonElement>>(JsonOpts);
        devices!.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Theory]
    [InlineData("switchable")]
    [InlineData("analog")]
    [InlineData("digital")]
    public async Task GetDevicesInRoom_By_Capability_Should_Return_200(string capability)
    {
        var roomId = $"rm-{Guid.NewGuid():N}".Substring(0, 12);
        await EnsureRoomAsync(roomId);
        await _client.PostAsync($"/rooms/{roomId}/devices/Light/cap-l", null);
        await _client.PostAsync($"/rooms/{roomId}/devices/rain_sensor/cap-r", null);
        await _client.PostAsync($"/rooms/{roomId}/devices/door_sensor/cap-d", null);

        var response = await _client.GetAsync($"/rooms/{roomId}/devices?capability={capability}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetDevicesInRoom_Unknown_Capability_Should_Return_400()
    {
        var roomId = $"rm-{Guid.NewGuid():N}".Substring(0, 12);
        await EnsureRoomAsync(roomId);

        var response = await _client.GetAsync($"/rooms/{roomId}/devices?capability=telepathy");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetDevicesInRoom_Missing_Room_Should_Fail()
    {
        var response = await _client.GetAsync("/rooms/ghost-room/devices");
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NotFound,
            HttpStatusCode.InternalServerError);
    }
}

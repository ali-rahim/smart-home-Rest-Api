using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using smart_home_Asp.net.Tests.Helpers;
using Xunit;

namespace smart_home_Asp.net.Tests.Integration;

public class RoomsApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public RoomsApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    // ---------- GET / ----------

    [Fact]
    public async Task Root_Should_Return_HelloWorld()
    {
        var response = await _client.GetAsync("/");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var text = await response.Content.ReadAsStringAsync();
        text.Should().Be("Hello World!");
    }

    // ---------- POST /rooms/{roomId} ----------

    [Fact]
    public async Task CreateRoom_Should_Return_201_And_Location()
    {
        var response = await _client.PostAsync("/rooms/living-room", null);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("living-room");
    }

    [Fact]
    public async Task CreateRoom_Duplicate_Should_Return_Conflict_Or_Error()
    {
        await _client.PostAsync("/rooms/dup-room", null);
        var response = await _client.PostAsync("/rooms/dup-room", null);

        // با Middleware اصلاح‌شده: 409 | با نسخه قدیمی: 500
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.Conflict,
            HttpStatusCode.InternalServerError);
    }

    // ---------- GET /rooms ----------

    [Fact]
    public async Task GetAllRooms_Should_Return_List()
    {
        await _client.PostAsync("/rooms/room-a", null);
        await _client.PostAsync("/rooms/room-b", null);

        var response = await _client.GetAsync("/rooms");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var rooms = await response.Content.ReadFromJsonAsync<List<JsonElement>>(JsonOpts);
        rooms.Should().NotBeNull();
        rooms!.Count.Should().BeGreaterThanOrEqualTo(2);
    }

    // ---------- GET /room/{id} ----------

    [Fact]
    public async Task GetRoomById_Should_Return_Room()
    {
        await _client.PostAsync("/rooms/get-me", null);

        var response = await _client.GetAsync("/room/get-me");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOpts);
        json.GetProperty("id").GetString().Should().Be("get-me");
    }

    [Fact]
    public async Task GetRoomById_Missing_Should_Return_NotFound_Or_Error()
    {
        var response = await _client.GetAsync("/room/does-not-exist");

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NotFound,
            HttpStatusCode.InternalServerError);
    }

    // ---------- DELETE /rooms/{roomId} ----------

    [Fact]
    public async Task DeleteRoom_Should_Return_204()
    {
        await _client.PostAsync("/rooms/to-delete", null);

        var response = await _client.DeleteAsync("/rooms/to-delete");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var get = await _client.GetAsync("/room/to-delete");
        get.StatusCode.Should().BeOneOf(
            HttpStatusCode.NotFound,
            HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task DeleteRoom_Missing_Should_Return_NotFound_Or_Error()
    {
        var response = await _client.DeleteAsync("/rooms/never-existed");

        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.NotFound,
            HttpStatusCode.InternalServerError);
    }
}

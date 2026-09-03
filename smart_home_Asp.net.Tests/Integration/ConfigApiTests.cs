using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using smart_home_Asp.net.Tests.Helpers;
using Xunit;

namespace smart_home_Asp.net.Tests.Integration;

public class ConfigApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ConfigApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetConfig_Should_Return_SmartHome_And_Storage_Options()
    {
        var response = await _client.GetAsync("/config");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        json.TryGetProperty("smartHome", out var smartHome).Should().BeTrue();
        json.TryGetProperty("storage", out var storage).Should().BeTrue();

        // مقادیر از appsettings.json
        smartHome.GetProperty("name").GetString().Should().Be("Ali Home");
        smartHome.GetProperty("automationEnabled").GetBoolean().Should().BeTrue();

        storage.GetProperty("provider").GetString().Should().Be("Json");
        storage.GetProperty("filePath").GetString().Should().Be("Data/smarthome.json");
        storage.GetProperty("autoSave").GetBoolean().Should().BeTrue();
    }
}

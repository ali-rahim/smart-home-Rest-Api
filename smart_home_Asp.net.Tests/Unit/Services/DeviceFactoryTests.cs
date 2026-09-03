using FluentAssertions;
using smart_home_Asp.net.Domain.Devices;
using smart_home_Asp.net.Domain.Devices.Base;
using smart_home_Asp.net.Services;
using Xunit;

namespace smart_home_Asp.net.Tests.Unit.Services;

public class DeviceFactoryTests
{
    [Theory]
    [InlineData(DeviceType.Light, typeof(Light))]
    [InlineData(DeviceType.fan, typeof(Fan))]
    [InlineData(DeviceType.dozdgir, typeof(SecurityAlarm))]
    [InlineData(DeviceType.door_sensor, typeof(door_sensor))]
    [InlineData(DeviceType.rain_sensor, typeof(Rain_sensor))]
    public void Create_Should_Return_Correct_Type(DeviceType type, Type expectedType)
    {
        var device = DeviceFactory.Create(type, "dev-1");

        device.Should().BeOfType(expectedType);
        device.Id.Should().Be("dev-1");
    }

    [Fact]
    public void Create_With_Invalid_Type_Should_Throw()
    {
        var invalid = (DeviceType)999;

        Action act = () => DeviceFactory.Create(invalid, "x");

        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("type");
    }
}

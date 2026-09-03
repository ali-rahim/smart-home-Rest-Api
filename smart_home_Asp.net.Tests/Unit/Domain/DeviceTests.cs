using FluentAssertions;
using smart_home_Asp.net.Domain.Devices;
using smart_home_Asp.net.Domain.Devices.ability_interfaces;
using smart_home_Asp.net.Domain.Devices.Base;
using Xunit;

namespace smart_home_Asp.net.Tests.Unit.Domain;

public class DeviceTests
{
    // ---------- Light ----------

    [Fact]
    public void Light_Should_Implement_Iswitchable()
    {
        var light = new Light("l1");
        light.Should().BeAssignableTo<Iswitchable>();
        light.Should().BeAssignableTo<Device>();
    }

    [Fact]
    public void Light_Should_Start_Off()
    {
        var light = new Light("l1");
        light.IsOn.Should().BeFalse();
    }

    [Fact]
    public void Light_Turn_on_Should_Set_IsOn_True()
    {
        var light = new Light("l1");
        light.Turn_on();
        light.IsOn.Should().BeTrue();
    }

    [Fact]
    public void Light_Turn_off_Should_Set_IsOn_False()
    {
        var light = new Light("l1");
        light.Turn_on();
        light.Turn_off();
        light.IsOn.Should().BeFalse();
    }

    [Fact]
    public void Light_Turn_on_When_Already_On_Should_Remain_On()
    {
        var light = new Light("l1");
        light.Turn_on();
        light.Turn_on();
        light.IsOn.Should().BeTrue();
    }

    // ---------- Fan ----------

    [Fact]
    public void Fan_Should_Implement_Iswitchable()
    {
        var fan = new Fan("f1");
        fan.Should().BeAssignableTo<Iswitchable>();
    }

    [Fact]
    public void Fan_Toggle_Works()
    {
        var fan = new Fan("f1");
        fan.IsOn.Should().BeFalse();
        fan.Turn_on();
        fan.IsOn.Should().BeTrue();
        fan.Turn_off();
        fan.IsOn.Should().BeFalse();
    }

    // ---------- SecurityAlarm ----------

    [Fact]
    public void SecurityAlarm_Should_Implement_Iswitchable()
    {
        var alarm = new SecurityAlarm("a1");
        alarm.Should().BeAssignableTo<Iswitchable>();
    }

    [Fact]
    public void SecurityAlarm_Toggle_Works()
    {
        var alarm = new SecurityAlarm("a1");
        alarm.IsOn.Should().BeFalse();
        alarm.Turn_on();
        alarm.IsOn.Should().BeTrue();
        alarm.Turn_off();
        alarm.IsOn.Should().BeFalse();
    }

    // ---------- door_sensor (digital) ----------

    [Fact]
    public void DoorSensor_Should_Implement_Idigital()
    {
        var sensor = new door_sensor("d1");
        sensor.Should().BeAssignableTo<Idigital>();
        sensor.Should().BeAssignableTo<Device>();
    }

    [Fact]
    public void DoorSensor_Default_Value_Is_True()
    {
        var sensor = new door_sensor("d1");
        sensor.get_value().Should().BeTrue();
        sensor.sensor_value.Should().BeTrue();
    }

    [Fact]
    public void DoorSensor_Value_Can_Be_Changed()
    {
        var sensor = new door_sensor("d1");
        sensor.sensor_value = false;
        sensor.get_value().Should().BeFalse();
    }

    // ---------- Rain_sensor (analog) ----------

    [Fact]
    public void RainSensor_Should_Implement_Ianalog()
    {
        var sensor = new Rain_sensor("r1");
        sensor.Should().BeAssignableTo<Ianalog>();
    }

    [Fact]
    public void RainSensor_Default_Value()
    {
        var sensor = new Rain_sensor("r1");
        sensor.get_value().Should().Be(25.85);
    }

    [Fact]
    public void RainSensor_Value_Can_Be_Changed()
    {
        var sensor = new Rain_sensor("r1");
        sensor.value_sensor = 99.5;
        sensor.get_value().Should().Be(99.5);
    }
}

using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using smart_home_Asp.net.Domain.Devices;
using smart_home_Asp.net.Domain.Devices.ability_interfaces;
using smart_home_Asp.net.Domain.Devices.Base;
using smart_home_Asp.net.Exceptions;
using smart_home_Asp.net.Services;
using Xunit;

namespace smart_home_Asp.net.Tests.Unit.Services;

public class DeviceManagerTests
{
    private readonly DeviceManager _sut = new(NullLogger<DeviceManager>.Instance);

    // ---------- Create ----------

    [Fact]
    public void CreateDevice_Should_Add_And_Return_Device()
    {
        var device = _sut.CreateDevice(DeviceType.Light, "light-1");

        device.Should().BeOfType<Light>();
        device.Id.Should().Be("light-1");
        _sut.GetAllDevices().Should().ContainSingle(d => d.Id == "light-1");
    }

    [Fact]
    public void CreateDevice_With_Empty_Id_Should_Throw()
    {
        _sut.Invoking(m => m.CreateDevice(DeviceType.Light, "  "))
            .Should().Throw<ArgumentException>()
            .WithParameterName("id");
    }

    [Fact]
    public void CreateDevice_With_Duplicate_Id_Should_Throw()
    {
        _sut.CreateDevice(DeviceType.Light, "light-1");

        _sut.Invoking(m => m.CreateDevice(DeviceType.fan, "light-1"))
            .Should().Throw<EntityAlreadyExistsException>()
            .Which.EntityId.Should().Be("light-1");
    }

    // ---------- Get / Remove ----------

    [Fact]
    public void GetDeviceById_Should_Return_Device()
    {
        _sut.CreateDevice(DeviceType.Light, "l1");
        var device = _sut.GetDeviceById("l1");
        device.Id.Should().Be("l1");
    }

    [Fact]
    public void GetDeviceById_When_Missing_Should_Throw()
    {
        _sut.Invoking(m => m.GetDeviceById("missing"))
            .Should().Throw<EntityNotFoundException>()
            .Which.EntityId.Should().Be("missing");
    }

    [Fact]
    public void RemoveDevice_Should_Remove()
    {
        _sut.CreateDevice(DeviceType.Light, "l1");
        _sut.RemoveDevice("l1");
        _sut.GetAllDevices().Should().BeEmpty();
    }

    [Fact]
    public void RemoveDevice_When_Missing_Should_Throw()
    {
        _sut.Invoking(m => m.RemoveDevice("x"))
            .Should().Throw<EntityNotFoundException>();
    }

    [Fact]
    public void GetAllDevices_Should_Return_All()
    {
        _sut.CreateDevice(DeviceType.Light, "l1");
        _sut.CreateDevice(DeviceType.fan, "f1");

        _sut.GetAllDevices().Should().HaveCount(2);
    }

    // ---------- Capability ----------

    [Fact]
    public void GetDevicesByCapability_Iswitchable_Should_Return_Only_Switchable()
    {
        _sut.CreateDevice(DeviceType.Light, "l1");
        _sut.CreateDevice(DeviceType.fan, "f1");
        _sut.CreateDevice(DeviceType.rain_sensor, "r1");
        _sut.CreateDevice(DeviceType.door_sensor, "d1");

        var switchables = _sut.GetDevicesByCapability<Iswitchable>().ToList();

        switchables.Should().HaveCount(2);
        switchables.Select(d => ((Device)d).Id).Should().BeEquivalentTo("l1", "f1");
    }

    [Fact]
    public void GetDevicesByCapability_Ianalog_Should_Return_Only_Analog()
    {
        _sut.CreateDevice(DeviceType.Light, "l1");
        _sut.CreateDevice(DeviceType.rain_sensor, "r1");

        var analogs = _sut.GetDevicesByCapability<Ianalog>().ToList();

        analogs.Should().ContainSingle()
            .Which.Should().BeOfType<Rain_sensor>();
    }

    [Fact]
    public void GetDevicesByCapability_Idigital_Should_Return_Only_Digital()
    {
        _sut.CreateDevice(DeviceType.door_sensor, "d1");
        _sut.CreateDevice(DeviceType.Light, "l1");

        var digitals = _sut.GetDevicesByCapability<Idigital>().ToList();

        digitals.Should().ContainSingle()
            .Which.Should().BeOfType<door_sensor>();
    }

    // ---------- Turn on/off ----------

    [Fact]
    public void Turn_on_oof_Should_Turn_On_When_Off()
    {
        var light = (Light)_sut.CreateDevice(DeviceType.Light, "l1");
        light.IsOn.Should().BeFalse();

        _sut.Turn_on_oof(light);

        light.IsOn.Should().BeTrue();
    }

    [Fact]
    public void Turn_on_oof_Should_Turn_Off_When_On()
    {
        var light = (Light)_sut.CreateDevice(DeviceType.Light, "l1");
        light.Turn_on();

        _sut.Turn_on_oof(light);

        light.IsOn.Should().BeFalse();
    }

    // ---------- Sensor value ----------

    [Fact]
    public void GetSensorValue_Digital_Should_Return_Bool()
    {
        var sensor = (door_sensor)_sut.CreateDevice(DeviceType.door_sensor, "d1");
        sensor.sensor_value = false;

        var value = _sut.GetSensorValue(sensor);

        value.Should().Be(false);
    }

    [Fact]
    public void GetSensorValue_Analog_Should_Return_Double()
    {
        var sensor = (Rain_sensor)_sut.CreateDevice(DeviceType.rain_sensor, "r1");
        sensor.value_sensor = 42.0;

        var value = _sut.GetSensorValue(sensor);

        value.Should().Be(42.0);
    }

    [Fact]
    public void GetSensorValue_NonSensor_Should_Throw()
    {
        var light = _sut.CreateDevice(DeviceType.Light, "l1");

        _sut.Invoking(m => m.GetSensorValue(light))
            .Should().Throw<InvalidOperationException>()
            .WithMessage("*not a sensor*");
    }
}

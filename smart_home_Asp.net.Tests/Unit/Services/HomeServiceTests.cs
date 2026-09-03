using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using smart_home_Asp.net.Domain.Devices;
using smart_home_Asp.net.Domain.Devices.ability_interfaces;
using smart_home_Asp.net.Domain.Devices.Base;
using smart_home_Asp.net.Domain.Entities;
using smart_home_Asp.net.Exceptions;
using smart_home_Asp.net.Services;
using Xunit;

namespace smart_home_Asp.net.Tests.Unit.Services;

public class HomeServiceTests
{
    private readonly Home _home = new("Ali Home");
    private readonly DeviceManager _deviceManager = new(NullLogger<DeviceManager>.Instance);
    private readonly RoomManager _roomManager = new(NullLogger<RoomManager>.Instance);
    private readonly HomeService _sut;

    public HomeServiceTests()
    {
        _sut = new HomeService(
            _deviceManager,
            _roomManager,
            _home,
            NullLogger<HomeService>.Instance);
    }

    // ---------- Room ----------

    [Fact]
    public void AddRoom_Should_Create_Room()
    {
        var room = _sut.AddRoom("living");
        room.Id.Should().Be("living");
        _sut.GetAllRooms().Should().ContainSingle(r => r.Id == "living");
    }

    [Fact]
    public void RemoveRoom_Should_Remove()
    {
        _sut.AddRoom("living");
        _sut.RemoveRoom("living");
        _sut.GetAllRooms().Should().BeEmpty();
    }

    [Fact]
    public void GetRoomById_Should_Return()
    {
        _sut.AddRoom("living");
        _sut.GetRoomById("living").Id.Should().Be("living");
    }

    // ---------- Device ----------

    [Fact]
    public void CreateDevice_Should_Create()
    {
        var device = _sut.CreateDevice(DeviceType.Light, "l1");
        device.Should().BeOfType<Light>();
        _sut.GetAllDevices().Should().ContainSingle(d => d.Id == "l1");
    }

    [Fact]
    public void CreateDeviceInRoom_Should_Create_And_Attach()
    {
        _sut.AddRoom("living");
        var device = _sut.CreateDeviceInRoom(DeviceType.Light, "l1", "living");

        device.Id.Should().Be("l1");
        _sut.GetDevicesInRoomByCapability<Iswitchable>("living")
            .Should().ContainSingle();
    }

    [Fact]
    public void CreateDeviceInRoom_When_Room_Missing_Should_Rollback_Device()
    {
        _sut.Invoking(s => s.CreateDeviceInRoom(DeviceType.Light, "l1", "no-room"))
            .Should().Throw<EntityNotFoundException>();

        // دستگاه نباید باقی مانده باشد (rollback)
        _sut.GetAllDevices().Should().BeEmpty();
    }

    [Fact]
    public void AddDeviceToRoom_Should_Attach_Existing_Device()
    {
        _sut.AddRoom("living");
        _sut.CreateDevice(DeviceType.fan, "f1");

        _sut.AddDeviceToRoom("f1", "living");

        _sut.GetDevicesInRoomByCapability<Iswitchable>("living")
            .Should().ContainSingle(d => ((Device)d).Id == "f1");
    }

    [Fact]
    public void RemoveDeviceFromRoom_Should_Detach()
    {
        _sut.AddRoom("living");
        _sut.CreateDeviceInRoom(DeviceType.Light, "l1", "living");

        _sut.RemoveDeviceFromRoom("living", "l1");

        _sut.GetDevicesInRoomByCapability<Device>("living").Should().BeEmpty();
        // دستگاه هنوز در DeviceManager هست
        _sut.GetDeviceById("l1").Should().NotBeNull();
    }

    [Fact]
    public void RemoveDeviceCompletely_Should_Remove_From_Manager_And_All_Rooms()
    {
        _sut.AddRoom("living");
        _sut.CreateDeviceInRoom(DeviceType.Light, "l1", "living");

        _sut.RemoveDeviceCompletely("l1");

        _sut.GetAllDevices().Should().BeEmpty();
        _sut.GetDevicesInRoomByCapability<Device>("living").Should().BeEmpty();
    }

    // ---------- Capabilities & Actions ----------

    [Fact]
    public void Turn_on_off_Should_Toggle_Light()
    {
        var light = (Light)_sut.CreateDevice(DeviceType.Light, "l1");
        light.IsOn.Should().BeFalse();

        _sut.Turn_on_off(light);
        light.IsOn.Should().BeTrue();

        _sut.Turn_on_off(light);
        light.IsOn.Should().BeFalse();
    }

    [Fact]
    public void get_Status_Digital_Should_Return_Value()
    {
        var sensor = (door_sensor)_sut.CreateDevice(DeviceType.door_sensor, "d1");
        sensor.sensor_value = true;

        var value = _sut.get_Status(sensor);
        value.Should().Be(true);
    }

    [Fact]
    public void get_Status_Analog_Should_Return_Value()
    {
        var sensor = (Rain_sensor)_sut.CreateDevice(DeviceType.rain_sensor, "r1");
        var value = _sut.get_Status(sensor);
        value.Should().Be(25.85);
    }

    [Fact]
    public void GetDevicesByCapability_Should_Filter()
    {
        _sut.CreateDevice(DeviceType.Light, "l1");
        _sut.CreateDevice(DeviceType.rain_sensor, "r1");

        _sut.GetDevicesByCapability<Iswitchable>().Should().ContainSingle();
        _sut.GetDevicesByCapability<Ianalog>().Should().ContainSingle();
    }

    // ---------- Constructor guards ----------

    [Fact]
    public void Constructor_Null_Dependencies_Should_Throw()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new HomeService(null!, _roomManager, _home, NullLogger<HomeService>.Instance));

        Assert.Throws<ArgumentNullException>(() =>
            new HomeService(_deviceManager, null!, _home, NullLogger<HomeService>.Instance));

        Assert.Throws<ArgumentNullException>(() =>
            new HomeService(_deviceManager, _roomManager, null!, NullLogger<HomeService>.Instance));

        Assert.Throws<ArgumentNullException>(() =>
            new HomeService(_deviceManager, _roomManager, _home, null!));
    }
}

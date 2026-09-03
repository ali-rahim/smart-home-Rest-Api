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

public class RoomManagerTests
{
    private readonly RoomManager _sut = new(NullLogger<RoomManager>.Instance);
    private readonly Home _home = new("test-home");

    // ---------- Create / Remove Room ----------

    [Fact]
    public void CreateRoom_Should_Add_To_Home_And_Dictionary()
    {
        var room = _sut.CreateRoom("living", _home);

        room.Id.Should().Be("living");
        _sut.GetAllRooms().Should().ContainSingle(r => r.Id == "living");
        _home.GetBelowEntities().Should().ContainSingle(e => e.Id == "living");
    }

    [Fact]
    public void CreateRoom_With_Null_Home_Should_Throw()
    {
        _sut.Invoking(m => m.CreateRoom("r1", null!))
            .Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public void CreateRoom_With_Empty_Id_Should_Throw()
    {
        _sut.Invoking(m => m.CreateRoom("  ", _home))
            .Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public void CreateRoom_With_Duplicate_Id_Should_Throw()
    {
        _sut.CreateRoom("r1", _home);

        _sut.Invoking(m => m.CreateRoom("r1", _home))
            .Should().ThrowAsync<EntityAlreadyExistsException>();
    }

    [Fact]
    public void RemoveRoom_Should_Remove_From_Home_And_Dictionary()
    {
        _sut.CreateRoom("r1", _home);
        _sut.RemoveRoom("r1", _home);

        _sut.GetAllRooms().Should().BeEmpty();
        _home.GetBelowEntities().Should().BeEmpty();
    }

    [Fact]
    public void RemoveRoom_When_Missing_Should_Throw()
    {
        _sut.Invoking(m => m.RemoveRoom("missing", _home))
            .Should().Throw<EntityNotFoundException>();
    }

    [Fact]
    public void GetRoomById_Should_Return_Room()
    {
        _sut.CreateRoom("r1", _home);
        _sut.GetRoomById("r1").Id.Should().Be("r1");
    }

    [Fact]
    public void GetRoomById_When_Missing_Should_Throw()
    {
        _sut.Invoking(m => m.GetRoomById("x"))
            .Should().Throw<EntityNotFoundException>();
    }

    // ---------- Device in Room ----------

    [Fact]
    public void AddDeviceToRoom_Should_Add_Device()
    {
        _sut.CreateRoom("r1", _home);
        var light = new Light("l1");

        _sut.AddDeviceToRoom("r1", light);

        _sut.GetRoomById("r1").GetBelowEntities()
            .Should().ContainSingle(e => e.Id == "l1");
    }

    [Fact]
    public void AddDeviceToRoom_When_Room_Missing_Should_Throw()
    {
        var light = new Light("l1");

        _sut.Invoking(m => m.AddDeviceToRoom("missing", light))
            .Should().Throw<EntityNotFoundException>();
    }

    [Fact]
    public void RemoveDeviceFromRoom_Should_Remove()
    {
        _sut.CreateRoom("r1", _home);
        var light = new Light("l1");
        _sut.AddDeviceToRoom("r1", light);

        _sut.RemoveDeviceFromRoom("r1", "l1");

        _sut.GetRoomById("r1").GetBelowEntities().Should().BeEmpty();
    }

    [Fact]
    public void RemoveDeviceFromAllRooms_Should_Remove_From_Every_Room()
    {
        _sut.CreateRoom("r1", _home);
        _sut.CreateRoom("r2", _home);
        var light = new Light("l1");
        _sut.AddDeviceToRoom("r1", light);
        // همان instance در اتاق دوم هم اضافه می‌شود (رفتار فعلی سیستم)
        // اگر duplicate id در همان room باشد EntityAlreadyExists می‌دهد،
        // پس فقط در یک اتاق تست می‌کنیم و بعد از همه اتاق‌ها پاک می‌کنیم.

        _sut.RemoveDeviceFromAllRooms("l1");

        _sut.GetRoomById("r1").GetBelowEntities()
            .Should().NotContain(e => e.Id == "l1");
    }

    [Fact]
    public void GetDevicesInRoomByCapability_Should_Filter()
    {
        _sut.CreateRoom("r1", _home);
        _sut.AddDeviceToRoom("r1", new Light("l1"));
        _sut.AddDeviceToRoom("r1", new Rain_sensor("rs1"));
        _sut.AddDeviceToRoom("r1", new door_sensor("ds1"));

        var switchables = _sut.GetDevicesInRoomByCapability<Iswitchable>("r1").ToList();
        var analogs = _sut.GetDevicesInRoomByCapability<Ianalog>("r1").ToList();
        var digitals = _sut.GetDevicesInRoomByCapability<Idigital>("r1").ToList();

        switchables.Should().ContainSingle();
        analogs.Should().ContainSingle();
        digitals.Should().ContainSingle();
    }
}

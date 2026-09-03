using FluentAssertions;
using smart_home_Asp.net.Domain.Devices;
using smart_home_Asp.net.Domain.Devices.Base;
using smart_home_Asp.net.Domain.Entities;
using smart_home_Asp.net.Exceptions;
using Xunit;

namespace smart_home_Asp.net.Tests.Unit.Domain;

public class EntityTests
{
    [Fact]
    public void Entity_Should_Set_Id_From_Constructor()
    {
        var room = new Room("living-room");

        room.Id.Should().Be("living-room");
    }

    [Fact]
    public void Home_Should_Only_Accept_Room_As_Child()
    {
        var home = new Home("my-home");
        var room = new Room("bedroom");
        var light = new Light("lamp-1");

        home.Invoking(h => h.AddBelowEntity(room))
            .Should().NotThrow();

        home.Invoking(h => h.AddBelowEntity(light))
            .Should().Throw<InvalidChildException>()
            .Where(ex => ex.ParentId == "my-home" && ex.ChildId == "lamp-1");
    }

    [Fact]
    public void Room_Should_Only_Accept_Device_As_Child()
    {
        var room = new Room("kitchen");
        var fan = new Fan("fan-1");
        var anotherRoom = new Room("bathroom");

        room.Invoking(r => r.AddBelowEntity(fan))
            .Should().NotThrow();

        room.Invoking(r => r.AddBelowEntity(anotherRoom))
            .Should().Throw<InvalidChildException>();
    }

    [Fact]
    public void AddBelowEntity_Should_Throw_When_Child_Is_Null()
    {
        var home = new Home("home");

        home.Invoking(h => h.AddBelowEntity(null!))
            .Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddBelowEntity_Should_Throw_When_Duplicate_Id()
    {
        var home = new Home("home");
        home.AddBelowEntity(new Room("r1"));

        home.Invoking(h => h.AddBelowEntity(new Room("r1")))
            .Should().Throw<EntityAlreadyExistsException>()
            .Which.EntityId.Should().Be("r1");
    }

    [Fact]
    public void RemoveBelowEntities_Should_Throw_When_Not_Found()
    {
        var home = new Home("home");

        home.Invoking(h => h.RemoveBelowEntities("missing"))
            .Should().Throw<EntityNotFoundException>()
            .Which.EntityId.Should().Be("missing");
    }

    [Fact]
    public void RemoveBelowEntities_Should_Throw_When_Id_Is_Empty()
    {
        var home = new Home("home");

        home.Invoking(h => h.RemoveBelowEntities("  "))
            .Should().Throw<ArgumentException>();
    }

    [Fact]
    public void FindEntity_Should_Return_Entity_When_Exists()
    {
        var home = new Home("home");
        var room = new Room("r1");
        home.AddBelowEntity(room);

        var found = home.FindEntity("r1");

        found.Should().BeSameAs(room);
    }

    [Fact]
    public void FindEntity_Should_Throw_When_Not_Found()
    {
        var home = new Home("home");

        home.Invoking(h => h.FindEntity("x"))
            .Should().Throw<EntityNotFoundException>();
    }

    [Fact]
    public void GetBelowEntities_Should_Return_ReadOnly_List()
    {
        var home = new Home("home");
        home.AddBelowEntity(new Room("r1"));

        var children = home.GetBelowEntities();

        children.Should().HaveCount(1);
        children.Should().BeAssignableTo<IReadOnlyList<Entity>>();
    }
}

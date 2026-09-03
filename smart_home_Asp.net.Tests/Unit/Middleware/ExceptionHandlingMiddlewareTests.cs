using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using smart_home_Asp.net.Exceptions;
using smart_home_Asp.net.YourProjectName.Middleware;
using Xunit;

namespace smart_home_Asp.net.Tests.Unit.Middleware;

/// <summary>
/// این تست‌ها رفتار ایده‌آل Middleware را بررسی می‌کنند.
/// اگر هنوز نسخه قدیمی (همیشه ۵۰۰) داری، اول Middleware را با نسخه‌ی اصلاح‌شده عوض کن.
/// </summary>
public class ExceptionHandlingMiddlewareTests
{
    private static ExceptionHandlingMiddleware CreateSut(RequestDelegate next)
        => new(next, NullLogger<ExceptionHandlingMiddleware>.Instance);

    private static async Task<(int StatusCode, ProblemDetails? Body)> InvokeAsync(Exception exception)
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var sut = CreateSut(_ => throw exception);
        await sut.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var json = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var body = string.IsNullOrWhiteSpace(json)
            ? null
            : JsonSerializer.Deserialize<ProblemDetails>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return (context.Response.StatusCode, body);
    }

    [Fact]
    public async Task EntityNotFoundException_Should_Return_404()
    {
        var (status, body) = await InvokeAsync(new EntityNotFoundException("room-1"));

        status.Should().Be((int)HttpStatusCode.NotFound);
        body.Should().NotBeNull();
        body!.Status.Should().Be(404);
        body.Title.Should().Be("Not Found");
        body.Detail.Should().Contain("room-1");
    }

    [Fact]
    public async Task EntityAlreadyExistsException_Should_Return_409()
    {
        var (status, body) = await InvokeAsync(new EntityAlreadyExistsException("dev-1"));

        status.Should().Be((int)HttpStatusCode.Conflict);
        body!.Status.Should().Be(409);
        body.Title.Should().Be("Conflict");
        body.Detail.Should().Contain("dev-1");
    }

    [Fact]
    public async Task InvalidChildException_Should_Return_400()
    {
        var (status, body) = await InvokeAsync(new InvalidChildException("home", "device"));

        status.Should().Be((int)HttpStatusCode.BadRequest);
        body!.Status.Should().Be(400);
        body.Title.Should().Be("Bad Request");
    }

    [Fact]
    public async Task ArgumentNullException_Should_Return_400()
    {
        var (status, _) = await InvokeAsync(new ArgumentNullException("param"));
        status.Should().Be(400);
    }

    [Fact]
    public async Task ArgumentException_Should_Return_400()
    {
        var (status, _) = await InvokeAsync(new ArgumentException("bad id"));
        status.Should().Be(400);
    }

    [Fact]
    public async Task InvalidOperationException_Should_Return_400()
    {
        var (status, _) = await InvokeAsync(new InvalidOperationException("not a sensor"));
        status.Should().Be(400);
    }

    [Fact]
    public async Task UnknownException_Should_Return_500()
    {
        var (status, body) = await InvokeAsync(new Exception("boom"));

        status.Should().Be(500);
        body!.Title.Should().Be("Internal Server Error");
    }

    [Fact]
    public async Task When_No_Exception_Should_Call_Next_And_Not_Change_Status()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var called = false;

        var sut = CreateSut(ctx =>
        {
            called = true;
            ctx.Response.StatusCode = 200;
            return Task.CompletedTask;
        });

        await sut.InvokeAsync(context);

        called.Should().BeTrue();
        context.Response.StatusCode.Should().Be(200);
    }
}

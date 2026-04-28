using ArchLens.Report.Api.Middlewares;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace ArchLens.Report.Tests.Api.Middlewares;

public class CorrelationIdMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_WithExistingCorrelationId_ShouldPreserveIt()
    {
        var existingId = Guid.NewGuid().ToString();
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Correlation-Id"] = existingId;

        var nextCalled = false;
        var middleware = new CorrelationIdMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        await middleware.InvokeAsync(context);

        nextCalled.Should().BeTrue();
        context.Response.Headers["X-Correlation-Id"].ToString().Should().Be(existingId);
        context.Items["X-Correlation-Id"]!.ToString().Should().Be(existingId);
    }

    [Fact]
    public async Task InvokeAsync_WithoutCorrelationId_ShouldGenerateNew()
    {
        var context = new DefaultHttpContext();

        var middleware = new CorrelationIdMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context);

        context.Response.Headers["X-Correlation-Id"].ToString().Should().NotBeNullOrEmpty();
        context.Items["X-Correlation-Id"]!.ToString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task InvokeAsync_WithEmptyCorrelationId_ShouldGenerateNew()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Correlation-Id"] = "   ";

        var middleware = new CorrelationIdMiddleware(_ => Task.CompletedTask);

        await middleware.InvokeAsync(context);

        var correlationId = context.Response.Headers["X-Correlation-Id"].ToString();
        correlationId.Should().NotBe("   ");
        correlationId.Should().NotBeNullOrEmpty();
    }
}

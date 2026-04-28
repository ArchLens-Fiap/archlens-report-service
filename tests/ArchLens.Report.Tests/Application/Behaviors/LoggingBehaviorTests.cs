using ArchLens.Report.Application.Behaviors;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace ArchLens.Report.Tests.Application.Behaviors;

public class LoggingBehaviorTests
{
    private readonly ILogger<LoggingBehavior<TestRequest, TestResponse>> _logger =
        Substitute.For<ILogger<LoggingBehavior<TestRequest, TestResponse>>>();

    [Fact]
    public async Task Handle_ShouldCallNextAndReturnResponse()
    {
        var behavior = new LoggingBehavior<TestRequest, TestResponse>(_logger);
        var expectedResponse = new TestResponse("ok");

        var result = await behavior.Handle(
            new TestRequest("test"),
            () => Task.FromResult(expectedResponse),
            CancellationToken.None);

        result.Should().Be(expectedResponse);
    }

    [Fact]
    public async Task Handle_ShouldLogRequestName()
    {
        var behavior = new LoggingBehavior<TestRequest, TestResponse>(_logger);

        await behavior.Handle(
            new TestRequest("test"),
            () => Task.FromResult(new TestResponse("ok")),
            CancellationToken.None);

        _logger.ReceivedCalls().Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_WhenNextThrows_ShouldPropagateException()
    {
        var behavior = new LoggingBehavior<TestRequest, TestResponse>(_logger);

        var act = () => behavior.Handle(
            new TestRequest("test"),
            () => throw new InvalidOperationException("Handler failed"),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Handler failed");
    }

    public record TestRequest(string Value) : IRequest<TestResponse>;
    public record TestResponse(string Result);
}

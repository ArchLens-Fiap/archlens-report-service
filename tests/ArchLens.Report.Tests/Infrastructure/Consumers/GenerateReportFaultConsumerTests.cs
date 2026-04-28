using ArchLens.Contracts.Events;
using ArchLens.Report.Infrastructure.Consumers;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace ArchLens.Report.Tests.Infrastructure.Consumers;

public class GenerateReportFaultConsumerTests
{
    private readonly ILogger<GenerateReportFaultConsumer> _logger = Substitute.For<ILogger<GenerateReportFaultConsumer>>();
    private readonly GenerateReportFaultConsumer _consumer;

    public GenerateReportFaultConsumerTests()
    {
        _consumer = new GenerateReportFaultConsumer(_logger);
    }

    [Fact]
    public async Task Consume_ShouldPublishReportFailedEvent()
    {
        var originalCommand = new GenerateReportCommand
        {
            AnalysisId = Guid.NewGuid(),
            DiagramId = Guid.NewGuid(),
            ResultJson = "{}",
            ProvidersUsed = ["openai"],
            ProcessingTimeMs = 100,
            Timestamp = DateTime.UtcNow
        };

        var faultException = Substitute.For<ExceptionInfo>();
        faultException.Message.Returns("Something failed");

        var faultMessage = Substitute.For<Fault<GenerateReportCommand>>();
        faultMessage.Message.Returns(originalCommand);
        faultMessage.Exceptions.Returns([faultException]);

        var context = Substitute.For<ConsumeContext<Fault<GenerateReportCommand>>>();
        context.Message.Returns(faultMessage);

        await _consumer.Consume(context);

        await context.Received(1).Publish(
            Arg.Is<ReportFailedEvent>(e =>
                e.AnalysisId == originalCommand.AnalysisId &&
                e.DiagramId == originalCommand.DiagramId &&
                e.ErrorMessage == "Something failed"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Consume_NoExceptions_ShouldUseUnknownError()
    {
        var originalCommand = new GenerateReportCommand
        {
            AnalysisId = Guid.NewGuid(),
            DiagramId = Guid.NewGuid(),
            ResultJson = "{}",
            ProvidersUsed = [],
            ProcessingTimeMs = 100,
            Timestamp = DateTime.UtcNow
        };

        var faultMessage = Substitute.For<Fault<GenerateReportCommand>>();
        faultMessage.Message.Returns(originalCommand);
        faultMessage.Exceptions.Returns([]);

        var context = Substitute.For<ConsumeContext<Fault<GenerateReportCommand>>>();
        context.Message.Returns(faultMessage);

        await _consumer.Consume(context);

        await context.Received(1).Publish(
            Arg.Is<ReportFailedEvent>(e => e.ErrorMessage == "Unknown error"),
            Arg.Any<CancellationToken>());
    }
}

using System.Text.Json;
using ArchLens.Contracts.Events;
using ArchLens.Report.Domain.Entities.ReportEntities;
using ArchLens.Report.Domain.Interfaces.ReportInterfaces;
using ArchLens.Report.Infrastructure.Consumers;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace ArchLens.Report.Tests.Infrastructure.Consumers;

public class GenerateReportConsumerTests
{
    private readonly IReportRepository _repository = Substitute.For<IReportRepository>();
    private readonly ILogger<GenerateReportConsumer> _logger = Substitute.For<ILogger<GenerateReportConsumer>>();
    private readonly GenerateReportConsumer _consumer;

    public GenerateReportConsumerTests()
    {
        _consumer = new GenerateReportConsumer(_repository, _logger);
    }

    private static string CreateValidResultJson(
        int componentCount = 1,
        int riskCount = 1,
        double confidence = 0.85)
    {
        var result = new
        {
            Components = Enumerable.Range(0, componentCount).Select(i => new
            {
                Name = $"Service{i}",
                Type = "microservice",
                Description = $"Description {i}",
                Confidence = 0.9
            }).ToArray(),
            Connections = new[]
            {
                new { Source = "A", Target = "B", Type = "HTTP", Description = "REST call" }
            },
            Risks = Enumerable.Range(0, riskCount).Select(i => new
            {
                Title = $"Risk{i}",
                Description = "Risk description",
                Severity = "high",
                Category = "security",
                Mitigation = "Fix it"
            }).ToArray(),
            Recommendations = new[] { "Add caching" },
            Scores = new
            {
                Scalability = 7.0,
                Security = 8.0,
                Reliability = 6.0,
                Maintainability = 7.0
            },
            Confidence = confidence
        };
        return JsonSerializer.Serialize(result);
    }

    private static ConsumeContext<GenerateReportCommand> CreateConsumeContext(GenerateReportCommand command)
    {
        var context = Substitute.For<ConsumeContext<GenerateReportCommand>>();
        context.Message.Returns(command);
        context.CancellationToken.Returns(CancellationToken.None);
        return context;
    }

    [Fact]
    public async Task Consume_ValidMessage_ShouldAddReportToRepository()
    {
        var command = new GenerateReportCommand
        {
            AnalysisId = Guid.NewGuid(),
            DiagramId = Guid.NewGuid(),
            ResultJson = CreateValidResultJson(),
            ProvidersUsed = ["openai"],
            ProcessingTimeMs = 1500,
            UserId = "user-123",
            Timestamp = DateTime.UtcNow
        };
        var context = CreateConsumeContext(command);

        await _consumer.Consume(context);

        await _repository.Received(1).AddAsync(Arg.Any<AnalysisReport>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Consume_ValidMessage_ShouldPublishReportGeneratedEvent()
    {
        var command = new GenerateReportCommand
        {
            AnalysisId = Guid.NewGuid(),
            DiagramId = Guid.NewGuid(),
            ResultJson = CreateValidResultJson(),
            ProvidersUsed = ["openai"],
            ProcessingTimeMs = 1500,
            Timestamp = DateTime.UtcNow
        };
        var context = CreateConsumeContext(command);

        await _consumer.Consume(context);

        await context.Received(1).Publish(
            Arg.Is<ReportGeneratedEvent>(e =>
                e.AnalysisId == command.AnalysisId &&
                e.DiagramId == command.DiagramId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Consume_WithNullComponentFields_ShouldUseDefaults()
    {
        var json = JsonSerializer.Serialize(new
        {
            Components = new[] { new { Name = (string?)null, Type = (string?)null, Description = (string?)null, Confidence = 0.5 } },
            Connections = Array.Empty<object>(),
            Risks = Array.Empty<object>(),
            Recommendations = Array.Empty<string>(),
            Scores = new { Scalability = 5.0, Security = 5.0, Reliability = 5.0, Maintainability = 5.0 },
            Confidence = 0.7
        });
        var command = new GenerateReportCommand
        {
            AnalysisId = Guid.NewGuid(),
            DiagramId = Guid.NewGuid(),
            ResultJson = json,
            ProvidersUsed = ["openai"],
            ProcessingTimeMs = 100,
            Timestamp = DateTime.UtcNow
        };
        var context = CreateConsumeContext(command);

        await _consumer.Consume(context);

        await _repository.Received(1).AddAsync(
            Arg.Is<AnalysisReport>(r => r.Components.Count == 1 && r.Components[0].Name == "Unknown"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Consume_WithNullScores_ShouldUseDefaults()
    {
        var json = JsonSerializer.Serialize(new
        {
            Components = Array.Empty<object>(),
            Connections = Array.Empty<object>(),
            Risks = Array.Empty<object>(),
            Recommendations = Array.Empty<string>(),
            Scores = (object?)null,
            Confidence = 0.5
        });
        var command = new GenerateReportCommand
        {
            AnalysisId = Guid.NewGuid(),
            DiagramId = Guid.NewGuid(),
            ResultJson = json,
            ProvidersUsed = ["openai"],
            ProcessingTimeMs = 100,
            Timestamp = DateTime.UtcNow
        };
        var context = CreateConsumeContext(command);

        await _consumer.Consume(context);

        await _repository.Received(1).AddAsync(
            Arg.Is<AnalysisReport>(r => r.Scores.Scalability == 5 && r.Scores.Security == 5),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Consume_WithNullRiskFields_ShouldUseDefaults()
    {
        var json = JsonSerializer.Serialize(new
        {
            Components = Array.Empty<object>(),
            Connections = Array.Empty<object>(),
            Risks = new[] { new { Title = (string?)null, Description = (string?)null, Severity = (string?)null, Category = (string?)null, Mitigation = (string?)null } },
            Recommendations = (List<string>?)null,
            Scores = new { Scalability = 5.0, Security = 5.0, Reliability = 5.0, Maintainability = 5.0 },
            Confidence = 0.5
        });
        var command = new GenerateReportCommand
        {
            AnalysisId = Guid.NewGuid(),
            DiagramId = Guid.NewGuid(),
            ResultJson = json,
            ProvidersUsed = ["openai"],
            ProcessingTimeMs = 100,
            Timestamp = DateTime.UtcNow
        };
        var context = CreateConsumeContext(command);

        await _consumer.Consume(context);

        await _repository.Received(1).AddAsync(
            Arg.Is<AnalysisReport>(r => r.Risks.Count == 1 && r.Risks[0].Title == "Unknown Risk"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Consume_InvalidJson_ShouldThrow()
    {
        var command = new GenerateReportCommand
        {
            AnalysisId = Guid.NewGuid(),
            DiagramId = Guid.NewGuid(),
            ResultJson = "not-valid-json",
            ProvidersUsed = ["openai"],
            ProcessingTimeMs = 100,
            Timestamp = DateTime.UtcNow
        };
        var context = CreateConsumeContext(command);

        var act = () => _consumer.Consume(context);

        await act.Should().ThrowAsync<JsonException>();
    }

    [Fact]
    public async Task Consume_WithUserId_ShouldPassToReport()
    {
        var command = new GenerateReportCommand
        {
            AnalysisId = Guid.NewGuid(),
            DiagramId = Guid.NewGuid(),
            ResultJson = CreateValidResultJson(),
            ProvidersUsed = ["openai"],
            ProcessingTimeMs = 100,
            UserId = "user-abc",
            Timestamp = DateTime.UtcNow
        };
        var context = CreateConsumeContext(command);

        await _consumer.Consume(context);

        await _repository.Received(1).AddAsync(
            Arg.Is<AnalysisReport>(r => r.UserId == "user-abc"),
            Arg.Any<CancellationToken>());
    }
}

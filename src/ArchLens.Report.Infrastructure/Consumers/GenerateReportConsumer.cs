using System.Text.Json;
using ArchLens.Contracts.Events;
using ArchLens.Report.Domain.Entities.ReportEntities;
using ArchLens.Report.Domain.Interfaces.ReportInterfaces;
using ArchLens.Report.Domain.ValueObjects.Reports;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ArchLens.Report.Infrastructure.Consumers;

public sealed class GenerateReportConsumer(
    IReportRepository repository,
    ILogger<GenerateReportConsumer> logger) : IConsumer<GenerateReportCommand>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static string OrDefault(string? value, string fallback = "Unknown")
        => string.IsNullOrWhiteSpace(value) ? fallback : value;

    public async Task Consume(ConsumeContext<GenerateReportCommand> context)
    {
        var msg = context.Message;

        logger.LogInformation(
            "Generating report for analysis {AnalysisId}, diagram {DiagramId}",
            msg.AnalysisId, msg.DiagramId);

        var result = JsonSerializer.Deserialize<AnalysisResultJson>(msg.ResultJson, JsonOptions)
            ?? throw new InvalidOperationException($"Failed to deserialize analysis result JSON for analysis {msg.AnalysisId}");

        var components = (result.Components ?? [])
            .Where(c => !string.IsNullOrWhiteSpace(c.Name))
            .Select(c => new IdentifiedComponent(
                OrDefault(c.Name),
                OrDefault(c.Type),
                c.Description ?? "",
                c.Confidence)).ToList();

        var connections = (result.Connections ?? [])
            .Where(c => !string.IsNullOrWhiteSpace(c.Source) && !string.IsNullOrWhiteSpace(c.Target))
            .Select(c => new IdentifiedConnection(
                OrDefault(c.Source),
                OrDefault(c.Target),
                OrDefault(c.Type),
                c.Description ?? "")).ToList();

        var risks = (result.Risks ?? [])
            .Where(r => !string.IsNullOrWhiteSpace(r.Title))
            .Select(r => new ArchitectureRisk(
                OrDefault(r.Title),
                r.Description ?? "",
                OrDefault(r.Severity, "Medium"),
                OrDefault(r.Category, "General"),
                r.Mitigation ?? "")).ToList();

        var scores = new ArchitectureScores(
            result.Scores?.Scalability ?? 5,
            result.Scores?.Security ?? 5,
            result.Scores?.Reliability ?? 5,
            result.Scores?.Maintainability ?? 5);

        var report = AnalysisReport.Create(
            msg.AnalysisId,
            msg.DiagramId,
            components,
            connections,
            risks,
            result.Recommendations ?? [],
            scores,
            result.Confidence,
            msg.ProvidersUsed.ToList(),
            msg.ProcessingTimeMs,
            msg.UserId);

        await repository.AddAsync(report, context.CancellationToken);

        logger.LogInformation(
            "Report {ReportId} generated: {ComponentCount} components, {RiskCount} risks, score {Score:F1}",
            report.Id, components.Count, risks.Count, report.OverallScore);

        await context.Publish(new ReportGeneratedEvent
        {
            ReportId = report.Id,
            AnalysisId = msg.AnalysisId,
            DiagramId = msg.DiagramId,
            Timestamp = DateTime.UtcNow
        }, context.CancellationToken);
    }
}

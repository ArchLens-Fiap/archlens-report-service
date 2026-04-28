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

    public async Task Consume(ConsumeContext<GenerateReportCommand> context)
    {
        var msg = context.Message;

        logger.LogInformation(
            "Generating report for analysis {AnalysisId}, diagram {DiagramId}",
            msg.AnalysisId, msg.DiagramId);

        var result = JsonSerializer.Deserialize<AnalysisResultJson>(msg.ResultJson, JsonOptions)
            ?? throw new InvalidOperationException($"Failed to deserialize analysis result JSON for analysis {msg.AnalysisId}");

        var components = (result.Components ?? []).Select(c =>
            new IdentifiedComponent(
                c.Name ?? "Unknown",
                c.Type ?? "Unknown",
                c.Description ?? "",
                c.Confidence)).ToList();

        var connections = (result.Connections ?? []).Select(c =>
            new IdentifiedConnection(
                c.Source ?? "Unknown",
                c.Target ?? "Unknown",
                c.Type ?? "Unknown",
                c.Description ?? "")).ToList();

        var risks = (result.Risks ?? []).Select(r =>
            new ArchitectureRisk(
                r.Title ?? "Unknown Risk",
                r.Description ?? "",
                r.Severity ?? "Medium",
                r.Category ?? "General",
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

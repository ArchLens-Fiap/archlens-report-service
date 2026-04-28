using ArchLens.Report.Domain.Entities.ReportEntities;
using ArchLens.Report.Domain.ValueObjects.Reports;
using ArchLens.Report.Infrastructure.Persistence.MongoDB.Documents;

namespace ArchLens.Report.Infrastructure.Persistence.MongoDB;

internal static class ReportDocumentMapper
{
    public static ReportDocument ToDocument(AnalysisReport report) => new()
    {
        Id = report.Id,
        AnalysisId = report.AnalysisId,
        DiagramId = report.DiagramId,
        UserId = report.UserId,
        Components = report.Components.Select(c => new ComponentDoc
        {
            Name = c.Name,
            Type = c.Type,
            Description = c.Description,
            Confidence = c.Confidence
        }).ToList(),
        Connections = report.Connections.Select(c => new ConnectionDoc
        {
            Source = c.Source,
            Target = c.Target,
            Type = c.Type,
            Description = c.Description
        }).ToList(),
        Risks = report.Risks.Select(r => new RiskDoc
        {
            Title = r.Title,
            Description = r.Description,
            Severity = r.Severity,
            Category = r.Category,
            Mitigation = r.Mitigation
        }).ToList(),
        Recommendations = report.Recommendations.ToList(),
        Scores = new ScoresDoc
        {
            Scalability = report.Scores.Scalability,
            Security = report.Scores.Security,
            Reliability = report.Scores.Reliability,
            Maintainability = report.Scores.Maintainability
        },
        OverallScore = report.OverallScore,
        Confidence = report.Confidence,
        ProvidersUsed = report.ProvidersUsed.ToList(),
        ProcessingTimeMs = report.ProcessingTimeMs,
        CreatedAt = report.CreatedAt
    };

    public static AnalysisReport ToDomain(ReportDocument doc)
    {
        var components = doc.Components.Select(c =>
            new IdentifiedComponent(c.Name, c.Type, c.Description, c.Confidence)).ToList();

        var connections = doc.Connections.Select(c =>
            new IdentifiedConnection(c.Source, c.Target, c.Type, c.Description)).ToList();

        var risks = doc.Risks.Select(r =>
            new ArchitectureRisk(r.Title, r.Description, r.Severity, r.Category, r.Mitigation)).ToList();

        var scores = new ArchitectureScores(
            doc.Scores.Scalability,
            doc.Scores.Security,
            doc.Scores.Reliability,
            doc.Scores.Maintainability);

        return AnalysisReport.Reconstitute(
            doc.Id,
            doc.AnalysisId,
            doc.DiagramId,
            components,
            connections,
            risks,
            doc.Recommendations,
            scores,
            doc.OverallScore,
            doc.Confidence,
            doc.ProvidersUsed,
            doc.ProcessingTimeMs,
            doc.CreatedAt,
            doc.UserId);
    }
}

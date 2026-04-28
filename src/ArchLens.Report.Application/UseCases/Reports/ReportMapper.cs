using ArchLens.Report.Application.Contracts.DTOs.ReportDTOs;
using ArchLens.Report.Domain.Entities.ReportEntities;

namespace ArchLens.Report.Application.UseCases.Reports;

internal static class ReportMapper
{
    public static ReportResponse ToResponse(AnalysisReport report) => new(
        report.Id,
        report.AnalysisId,
        report.DiagramId,
        report.Components.Select(c => new ComponentDto(c.Name, c.Type, c.Description, c.Confidence)).ToList(),
        report.Connections.Select(c => new ConnectionDto(c.Source, c.Target, c.Type, c.Description)).ToList(),
        report.Risks.Select(r => new RiskDto(r.Title, r.Description, r.Severity, r.Category, r.Mitigation)).ToList(),
        report.Recommendations,
        new ScoresDto(report.Scores.Scalability, report.Scores.Security, report.Scores.Reliability, report.Scores.Maintainability),
        report.OverallScore,
        report.Confidence,
        report.ProvidersUsed,
        report.ProcessingTimeMs,
        report.CreatedAt);
}

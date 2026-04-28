namespace ArchLens.Report.Application.Contracts.DTOs.ReportDTOs;

public record ReportResponse(
    Guid Id,
    Guid AnalysisId,
    Guid DiagramId,
    IReadOnlyList<ComponentDto> Components,
    IReadOnlyList<ConnectionDto> Connections,
    IReadOnlyList<RiskDto> Risks,
    IReadOnlyList<string> Recommendations,
    ScoresDto Scores,
    double OverallScore,
    double Confidence,
    IReadOnlyList<string> ProvidersUsed,
    long ProcessingTimeMs,
    DateTime CreatedAt);

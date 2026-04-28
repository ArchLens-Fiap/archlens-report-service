namespace ArchLens.Report.Application.Contracts.DTOs.ReportDTOs;

public record ReportSummaryResponse(
    Guid Id,
    Guid AnalysisId,
    Guid DiagramId,
    double OverallScore,
    double Confidence,
    int ComponentCount,
    int RiskCount,
    IReadOnlyList<string> ProvidersUsed,
    DateTime CreatedAt);

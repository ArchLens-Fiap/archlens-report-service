namespace ArchLens.Report.Domain.Interfaces.ReportInterfaces;

public record AdminReportMetrics(
    int TotalReports,
    double AverageOverallScore,
    Dictionary<string, int> ProviderUsage,
    ScoreAverages AverageScores);

public record ScoreAverages(
    double Scalability,
    double Security,
    double Reliability,
    double Maintainability);

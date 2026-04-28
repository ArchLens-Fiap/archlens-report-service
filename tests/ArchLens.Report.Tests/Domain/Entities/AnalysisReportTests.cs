using ArchLens.Report.Domain.Entities.ReportEntities;
using ArchLens.Report.Domain.ValueObjects.Reports;
using FluentAssertions;

namespace ArchLens.Report.Tests.Domain.Entities;

public class AnalysisReportTests
{
    private static ArchitectureScores CreateScores(double s = 7, double sec = 8, double r = 6, double m = 7)
        => new(s, sec, r, m);

    private static List<IdentifiedComponent> CreateComponents() =>
    [
        new("API Gateway", "gateway", "Routes traffic", 0.9),
        new("User Service", "microservice", "Handles users", 0.85),
    ];

    private static List<IdentifiedConnection> CreateConnections() =>
    [
        new("API Gateway", "User Service", "HTTP", "REST calls"),
    ];

    private static List<ArchitectureRisk> CreateRisks() =>
    [
        new("SPOF", "Single point of failure", "high", "reliability", "Add redundancy"),
    ];

    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        var analysisId = Guid.NewGuid();
        var diagramId = Guid.NewGuid();

        var report = AnalysisReport.Create(
            analysisId, diagramId,
            CreateComponents(), CreateConnections(), CreateRisks(),
            ["Add caching"], CreateScores(), 0.85,
            ["openai", "gemini"], 1500);

        report.AnalysisId.Should().Be(analysisId);
        report.DiagramId.Should().Be(diagramId);
        report.Components.Should().HaveCount(2);
        report.Connections.Should().HaveCount(1);
        report.Risks.Should().HaveCount(1);
        report.Recommendations.Should().ContainSingle();
        report.Confidence.Should().Be(0.85);
        report.ProvidersUsed.Should().HaveCount(2);
        report.ProcessingTimeMs.Should().Be(1500);
        report.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithEmptyAnalysisId_ShouldThrow()
    {
        var act = () => AnalysisReport.Create(
            Guid.Empty, Guid.NewGuid(),
            CreateComponents(), CreateConnections(), CreateRisks(),
            [], CreateScores(), 0.5, [], 100);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithEmptyDiagramId_ShouldThrow()
    {
        var act = () => AnalysisReport.Create(
            Guid.NewGuid(), Guid.Empty,
            CreateComponents(), CreateConnections(), CreateRisks(),
            [], CreateScores(), 0.5, [], 100);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_Confidence_ShouldClampBetween0And1()
    {
        var report = AnalysisReport.Create(
            Guid.NewGuid(), Guid.NewGuid(),
            CreateComponents(), CreateConnections(), CreateRisks(),
            [], CreateScores(), 1.5, ["openai"], 100);

        report.Confidence.Should().Be(1.0);
    }

    [Fact]
    public void OverallScore_ShouldBeAverage_OfAllScores()
    {
        var report = AnalysisReport.Create(
            Guid.NewGuid(), Guid.NewGuid(),
            CreateComponents(), CreateConnections(), CreateRisks(),
            [], CreateScores(8, 6, 7, 7), 0.8, ["openai"], 100);

        report.OverallScore.Should().Be(7.0);
    }

    [Fact]
    public void Reconstitute_ShouldPreserve_AllFields()
    {
        var id = Guid.NewGuid();
        var analysisId = Guid.NewGuid();
        var diagramId = Guid.NewGuid();
        var createdAt = new DateTime(2026, 3, 10, 12, 0, 0, DateTimeKind.Utc);

        var report = AnalysisReport.Reconstitute(
            id, analysisId, diagramId,
            CreateComponents(), CreateConnections(), CreateRisks(),
            ["rec1"], CreateScores(), 7.0, 0.9, ["openai"], 2000, createdAt);

        report.Id.Should().Be(id);
        report.AnalysisId.Should().Be(analysisId);
        report.CreatedAt.Should().Be(createdAt);
    }
}

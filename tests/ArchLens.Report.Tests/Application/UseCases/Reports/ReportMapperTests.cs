using ArchLens.Report.Application.UseCases.Reports;
using ArchLens.Report.Domain.Entities.ReportEntities;
using ArchLens.Report.Domain.ValueObjects.Reports;
using FluentAssertions;

namespace ArchLens.Report.Tests.Application.UseCases.Reports;

public class ReportMapperTests
{
    private static ArchitectureScores DefaultScores() => new(8.0, 7.0, 6.0, 9.0);

    private static AnalysisReport CreateReport(
        IReadOnlyList<IdentifiedComponent>? components = null,
        IReadOnlyList<IdentifiedConnection>? connections = null,
        IReadOnlyList<ArchitectureRisk>? risks = null,
        IReadOnlyList<string>? recommendations = null)
    {
        return AnalysisReport.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            components ?? [new("API GW", "gateway", "Routes requests", 0.95)],
            connections ?? [new("API GW", "Auth", "HTTP", "JWT validation")],
            risks ?? [new("SPOF", "Single point of failure", "high", "reliability", "Add replica")],
            recommendations ?? ["Use circuit breakers"],
            DefaultScores(),
            0.88,
            ["openai", "gemini"],
            1500);
    }

    [Fact]
    public void ToResponse_ShouldMapAllTopLevelFields()
    {
        var report = CreateReport();

        var response = ReportMapper.ToResponse(report);

        response.Id.Should().Be(report.Id);
        response.AnalysisId.Should().Be(report.AnalysisId);
        response.DiagramId.Should().Be(report.DiagramId);
        response.OverallScore.Should().Be(report.OverallScore);
        response.Confidence.Should().BeApproximately(0.88, 0.001);
        response.ProcessingTimeMs.Should().Be(1500);
        response.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void ToResponse_ShouldMapComponents()
    {
        var components = new List<IdentifiedComponent>
        {
            new("ServiceA", "microservice", "Handles A", 0.9),
            new("ServiceB", "microservice", "Handles B", 0.8),
        };
        var report = CreateReport(components: components);

        var response = ReportMapper.ToResponse(report);

        response.Components.Should().HaveCount(2);
        response.Components[0].Name.Should().Be("ServiceA");
        response.Components[0].Type.Should().Be("microservice");
        response.Components[0].Description.Should().Be("Handles A");
        response.Components[0].Confidence.Should().Be(0.9);
        response.Components[1].Name.Should().Be("ServiceB");
    }

    [Fact]
    public void ToResponse_ShouldMapConnections()
    {
        var connections = new List<IdentifiedConnection>
        {
            new("A", "B", "gRPC", "Internal call"),
            new("B", "C", "HTTP", "External call"),
        };
        var report = CreateReport(connections: connections);

        var response = ReportMapper.ToResponse(report);

        response.Connections.Should().HaveCount(2);
        response.Connections[0].Source.Should().Be("A");
        response.Connections[0].Target.Should().Be("B");
        response.Connections[0].Type.Should().Be("gRPC");
        response.Connections[0].Description.Should().Be("Internal call");
    }

    [Fact]
    public void ToResponse_ShouldMapRisks()
    {
        var risks = new List<ArchitectureRisk>
        {
            new("Risk 1", "Desc 1", "critical", "security", "Mitigation 1"),
            new("Risk 2", "Desc 2", "low", "performance", "Mitigation 2"),
        };
        var report = CreateReport(risks: risks);

        var response = ReportMapper.ToResponse(report);

        response.Risks.Should().HaveCount(2);
        response.Risks[0].Title.Should().Be("Risk 1");
        response.Risks[0].Severity.Should().Be("critical");
        response.Risks[0].Category.Should().Be("security");
        response.Risks[0].Mitigation.Should().Be("Mitigation 1");
        response.Risks[1].Title.Should().Be("Risk 2");
    }

    [Fact]
    public void ToResponse_ShouldMapRecommendations()
    {
        var recs = new List<string> { "Use cache", "Add circuit breaker", "Enable retry" };
        var report = CreateReport(recommendations: recs);

        var response = ReportMapper.ToResponse(report);

        response.Recommendations.Should().BeEquivalentTo(recs);
    }

    [Fact]
    public void ToResponse_ShouldMapScores()
    {
        var report = CreateReport();

        var response = ReportMapper.ToResponse(report);

        response.Scores.Scalability.Should().Be(8.0);
        response.Scores.Security.Should().Be(7.0);
        response.Scores.Reliability.Should().Be(6.0);
        response.Scores.Maintainability.Should().Be(9.0);
    }

    [Fact]
    public void ToResponse_EmptyCollections_ShouldMapToEmptyLists()
    {
        var report = AnalysisReport.Create(
            Guid.NewGuid(), Guid.NewGuid(),
            [], [], [], [],
            DefaultScores(), 0.5, ["openai"], 100);

        var response = ReportMapper.ToResponse(report);

        response.Components.Should().BeEmpty();
        response.Connections.Should().BeEmpty();
        response.Risks.Should().BeEmpty();
        response.Recommendations.Should().BeEmpty();
    }

    [Fact]
    public void ToResponse_ShouldMapProvidersUsed()
    {
        var report = CreateReport();

        var response = ReportMapper.ToResponse(report);

        response.ProvidersUsed.Should().Contain("openai");
        response.ProvidersUsed.Should().Contain("gemini");
        response.ProvidersUsed.Should().HaveCount(2);
    }
}

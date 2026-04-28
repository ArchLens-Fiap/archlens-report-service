using ArchLens.Report.Infrastructure.Persistence.MongoDB.Documents;
using FluentAssertions;

namespace ArchLens.Report.Tests.Infrastructure.Persistence;

public class ReportDocumentTests
{
    [Fact]
    public void ReportDocument_DefaultValues_ShouldHaveEmptyCollections()
    {
        var doc = new ReportDocument();

        doc.Id.Should().Be(Guid.Empty);
        doc.AnalysisId.Should().Be(Guid.Empty);
        doc.DiagramId.Should().Be(Guid.Empty);
        doc.UserId.Should().BeNull();
        doc.Components.Should().BeEmpty();
        doc.Connections.Should().BeEmpty();
        doc.Risks.Should().BeEmpty();
        doc.Recommendations.Should().BeEmpty();
        doc.Scores.Should().BeNull();
        doc.OverallScore.Should().Be(0);
        doc.Confidence.Should().Be(0);
        doc.ProvidersUsed.Should().BeEmpty();
        doc.ProcessingTimeMs.Should().Be(0);
        doc.CreatedAt.Should().Be(default);
    }

    [Fact]
    public void ReportDocument_SetProperties_ShouldRetainValues()
    {
        var id = Guid.NewGuid();
        var analysisId = Guid.NewGuid();
        var diagramId = Guid.NewGuid();
        var createdAt = new DateTime(2026, 3, 15, 10, 0, 0, DateTimeKind.Utc);

        var doc = new ReportDocument
        {
            Id = id,
            AnalysisId = analysisId,
            DiagramId = diagramId,
            UserId = "user-789",
            Components = [new ComponentDoc { Name = "Svc", Type = "api", Description = "desc", Confidence = 0.8 }],
            Connections = [new ConnectionDoc { Source = "A", Target = "B", Type = "HTTP", Description = "call" }],
            Risks = [new RiskDoc { Title = "R1", Description = "d", Severity = "low", Category = "perf", Mitigation = "fix" }],
            Recommendations = ["Cache more"],
            Scores = new ScoresDoc { Scalability = 9, Security = 8, Reliability = 7, Maintainability = 6 },
            OverallScore = 7.5,
            Confidence = 0.9,
            ProvidersUsed = ["openai", "gemini"],
            ProcessingTimeMs = 3000,
            CreatedAt = createdAt
        };

        doc.Id.Should().Be(id);
        doc.AnalysisId.Should().Be(analysisId);
        doc.DiagramId.Should().Be(diagramId);
        doc.UserId.Should().Be("user-789");
        doc.Components.Should().HaveCount(1);
        doc.Connections.Should().HaveCount(1);
        doc.Risks.Should().HaveCount(1);
        doc.Recommendations.Should().ContainSingle().Which.Should().Be("Cache more");
        doc.Scores.Scalability.Should().Be(9);
        doc.OverallScore.Should().Be(7.5);
        doc.Confidence.Should().Be(0.9);
        doc.ProvidersUsed.Should().HaveCount(2);
        doc.ProcessingTimeMs.Should().Be(3000);
        doc.CreatedAt.Should().Be(createdAt);
    }

    [Fact]
    public void ComponentDoc_ShouldStoreAllProperties()
    {
        var comp = new ComponentDoc
        {
            Name = "Gateway",
            Type = "gateway",
            Description = "API Gateway",
            Confidence = 0.95
        };

        comp.Name.Should().Be("Gateway");
        comp.Type.Should().Be("gateway");
        comp.Description.Should().Be("API Gateway");
        comp.Confidence.Should().Be(0.95);
    }

    [Fact]
    public void ConnectionDoc_ShouldStoreAllProperties()
    {
        var conn = new ConnectionDoc
        {
            Source = "ServiceA",
            Target = "ServiceB",
            Type = "gRPC",
            Description = "Internal communication"
        };

        conn.Source.Should().Be("ServiceA");
        conn.Target.Should().Be("ServiceB");
        conn.Type.Should().Be("gRPC");
        conn.Description.Should().Be("Internal communication");
    }

    [Fact]
    public void RiskDoc_ShouldStoreAllProperties()
    {
        var risk = new RiskDoc
        {
            Title = "Single Point of Failure",
            Description = "No redundancy",
            Severity = "critical",
            Category = "reliability",
            Mitigation = "Add replicas"
        };

        risk.Title.Should().Be("Single Point of Failure");
        risk.Description.Should().Be("No redundancy");
        risk.Severity.Should().Be("critical");
        risk.Category.Should().Be("reliability");
        risk.Mitigation.Should().Be("Add replicas");
    }

    [Fact]
    public void ScoresDoc_ShouldStoreAllProperties()
    {
        var scores = new ScoresDoc
        {
            Scalability = 8.5,
            Security = 7.0,
            Reliability = 9.0,
            Maintainability = 6.5
        };

        scores.Scalability.Should().Be(8.5);
        scores.Security.Should().Be(7.0);
        scores.Reliability.Should().Be(9.0);
        scores.Maintainability.Should().Be(6.5);
    }

    [Fact]
    public void ScoresDoc_DefaultValues_ShouldBeZero()
    {
        var scores = new ScoresDoc();

        scores.Scalability.Should().Be(0);
        scores.Security.Should().Be(0);
        scores.Reliability.Should().Be(0);
        scores.Maintainability.Should().Be(0);
    }
}

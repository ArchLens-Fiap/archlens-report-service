using ArchLens.Report.Domain.Entities.ReportEntities;
using ArchLens.Report.Domain.ValueObjects.Reports;
using ArchLens.Report.Infrastructure.Persistence.MongoDB;
using ArchLens.Report.Infrastructure.Persistence.MongoDB.Documents;
using FluentAssertions;

namespace ArchLens.Report.Tests.Infrastructure.Persistence;

public class ReportDocumentMapperTests
{
    private static AnalysisReport CreateDomainReport()
    {
        return AnalysisReport.Create(
            Guid.NewGuid(), Guid.NewGuid(),
            [new IdentifiedComponent("GW", "gateway", "Gateway desc", 0.9)],
            [new IdentifiedConnection("A", "B", "HTTP", "call")],
            [new ArchitectureRisk("Risk", "desc", "high", "security", "fix")],
            ["Use caching"],
            new ArchitectureScores(7, 8, 6, 7),
            0.85, ["openai", "gemini"], 1200, "user-123");
    }

    private static ReportDocument CreateDocument()
    {
        return new ReportDocument
        {
            Id = Guid.NewGuid(),
            AnalysisId = Guid.NewGuid(),
            DiagramId = Guid.NewGuid(),
            UserId = "user-456",
            Components =
            [
                new ComponentDoc { Name = "SvcA", Type = "microservice", Description = "Handles A", Confidence = 0.95 }
            ],
            Connections =
            [
                new ConnectionDoc { Source = "X", Target = "Y", Type = "gRPC", Description = "Internal" }
            ],
            Risks =
            [
                new RiskDoc { Title = "SPOF", Description = "Single point", Severity = "critical", Category = "reliability", Mitigation = "Add replica" }
            ],
            Recommendations = ["Add monitoring"],
            Scores = new ScoresDoc { Scalability = 8, Security = 7, Reliability = 9, Maintainability = 6 },
            OverallScore = 7.5,
            Confidence = 0.92,
            ProvidersUsed = ["gemini"],
            ProcessingTimeMs = 2000,
            CreatedAt = new DateTime(2026, 3, 10, 12, 0, 0, DateTimeKind.Utc)
        };
    }

    [Fact]
    public void ToDocument_ShouldMapAllFields()
    {
        var report = CreateDomainReport();

        var doc = ReportDocumentMapper.ToDocument(report);

        doc.Id.Should().Be(report.Id);
        doc.AnalysisId.Should().Be(report.AnalysisId);
        doc.DiagramId.Should().Be(report.DiagramId);
        doc.UserId.Should().Be("user-123");
        doc.Components.Should().HaveCount(1);
        doc.Components[0].Name.Should().Be("GW");
        doc.Connections.Should().HaveCount(1);
        doc.Connections[0].Source.Should().Be("A");
        doc.Risks.Should().HaveCount(1);
        doc.Risks[0].Title.Should().Be("Risk");
        doc.Recommendations.Should().ContainSingle().Which.Should().Be("Use caching");
        doc.Scores.Scalability.Should().Be(7);
        doc.OverallScore.Should().Be(report.OverallScore);
        doc.Confidence.Should().Be(0.85);
        doc.ProvidersUsed.Should().Contain("openai");
        doc.ProcessingTimeMs.Should().Be(1200);
    }

    [Fact]
    public void ToDomain_ShouldMapAllFields()
    {
        var doc = CreateDocument();

        var report = ReportDocumentMapper.ToDomain(doc);

        report.Id.Should().Be(doc.Id);
        report.AnalysisId.Should().Be(doc.AnalysisId);
        report.DiagramId.Should().Be(doc.DiagramId);
        report.UserId.Should().Be("user-456");
        report.Components.Should().HaveCount(1);
        report.Components[0].Name.Should().Be("SvcA");
        report.Connections.Should().HaveCount(1);
        report.Connections[0].Source.Should().Be("X");
        report.Risks.Should().HaveCount(1);
        report.Risks[0].Title.Should().Be("SPOF");
        report.Recommendations.Should().ContainSingle().Which.Should().Be("Add monitoring");
        report.Scores.Scalability.Should().Be(8);
        report.OverallScore.Should().Be(7.5);
        report.Confidence.Should().Be(0.92);
        report.ProvidersUsed.Should().Contain("gemini");
        report.ProcessingTimeMs.Should().Be(2000);
        report.CreatedAt.Should().Be(doc.CreatedAt);
    }

    [Fact]
    public void Roundtrip_ShouldPreserveAllData()
    {
        var original = CreateDomainReport();

        var doc = ReportDocumentMapper.ToDocument(original);
        var restored = ReportDocumentMapper.ToDomain(doc);

        restored.Id.Should().Be(original.Id);
        restored.AnalysisId.Should().Be(original.AnalysisId);
        restored.DiagramId.Should().Be(original.DiagramId);
        restored.Components.Should().HaveCount(original.Components.Count);
        restored.Connections.Should().HaveCount(original.Connections.Count);
        restored.Risks.Should().HaveCount(original.Risks.Count);
        restored.Recommendations.Should().BeEquivalentTo(original.Recommendations);
        restored.OverallScore.Should().Be(original.OverallScore);
        restored.Confidence.Should().Be(original.Confidence);
        restored.ProcessingTimeMs.Should().Be(original.ProcessingTimeMs);
    }

    [Fact]
    public void ToDocument_EmptyCollections_ShouldProduceEmptyLists()
    {
        var report = AnalysisReport.Create(
            Guid.NewGuid(), Guid.NewGuid(),
            [], [], [], [],
            new ArchitectureScores(5, 5, 5, 5),
            0.5, [], 100);

        var doc = ReportDocumentMapper.ToDocument(report);

        doc.Components.Should().BeEmpty();
        doc.Connections.Should().BeEmpty();
        doc.Risks.Should().BeEmpty();
        doc.Recommendations.Should().BeEmpty();
        doc.ProvidersUsed.Should().BeEmpty();
    }
}

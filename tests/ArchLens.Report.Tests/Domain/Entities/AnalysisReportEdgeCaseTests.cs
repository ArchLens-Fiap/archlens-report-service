using ArchLens.Report.Domain.Entities.ReportEntities;
using ArchLens.Report.Domain.ValueObjects.Reports;
using FluentAssertions;

namespace ArchLens.Report.Tests.Domain.Entities;

public class AnalysisReportEdgeCaseTests
{
    private static ArchitectureScores DefaultScores() => new(7, 8, 6, 7);

    [Fact]
    public void Create_WithNullComponents_ShouldThrow()
    {
        var act = () => AnalysisReport.Create(
            Guid.NewGuid(), Guid.NewGuid(),
            null!, [], [], [],
            DefaultScores(), 0.5, [], 100);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Create_WithNullConnections_ShouldThrow()
    {
        var act = () => AnalysisReport.Create(
            Guid.NewGuid(), Guid.NewGuid(),
            [], null!, [], [],
            DefaultScores(), 0.5, [], 100);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Create_WithNullRisks_ShouldThrow()
    {
        var act = () => AnalysisReport.Create(
            Guid.NewGuid(), Guid.NewGuid(),
            [], [], null!, [],
            DefaultScores(), 0.5, [], 100);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Create_WithNullRecommendations_ShouldThrow()
    {
        var act = () => AnalysisReport.Create(
            Guid.NewGuid(), Guid.NewGuid(),
            [], [], [], null!,
            DefaultScores(), 0.5, [], 100);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Create_WithNullScores_ShouldThrow()
    {
        var act = () => AnalysisReport.Create(
            Guid.NewGuid(), Guid.NewGuid(),
            [], [], [], [],
            null!, 0.5, [], 100);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Create_WithNullProvidersUsed_ShouldThrow()
    {
        var act = () => AnalysisReport.Create(
            Guid.NewGuid(), Guid.NewGuid(),
            [], [], [], [],
            DefaultScores(), 0.5, null!, 100);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Create_NegativeConfidence_ShouldClampToZero()
    {
        var report = AnalysisReport.Create(
            Guid.NewGuid(), Guid.NewGuid(),
            [], [], [], [],
            DefaultScores(), -0.5, [], 100);

        report.Confidence.Should().Be(0.0);
    }

    [Fact]
    public void Create_WithUserId_ShouldSetUserId()
    {
        var report = AnalysisReport.Create(
            Guid.NewGuid(), Guid.NewGuid(),
            [], [], [], [],
            DefaultScores(), 0.5, [], 100, "user-xyz");

        report.UserId.Should().Be("user-xyz");
    }

    [Fact]
    public void Create_WithoutUserId_ShouldHaveNullUserId()
    {
        var report = AnalysisReport.Create(
            Guid.NewGuid(), Guid.NewGuid(),
            [], [], [], [],
            DefaultScores(), 0.5, [], 100);

        report.UserId.Should().BeNull();
    }

    [Fact]
    public void Reconstitute_ShouldPreserveUserId()
    {
        var report = AnalysisReport.Reconstitute(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            [], [], [], [],
            DefaultScores(), 7.0, 0.9, [], 100,
            DateTime.UtcNow, "user-abc");

        report.UserId.Should().Be("user-abc");
    }

    [Fact]
    public void Create_CreatedAt_ShouldBeRecentUtc()
    {
        var before = DateTime.UtcNow;
        var report = AnalysisReport.Create(
            Guid.NewGuid(), Guid.NewGuid(),
            [], [], [], [],
            DefaultScores(), 0.5, [], 100);
        var after = DateTime.UtcNow;

        report.CreatedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }
}

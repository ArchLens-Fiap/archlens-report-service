using ArchLens.Report.Domain.Entities.ReportEntities;
using ArchLens.Report.Domain.ValueObjects.Reports;
using FluentAssertions;

namespace ArchLens.Report.Tests.Domain.Entities;

public class AnalysisReportEqualityTests
{
    private static ArchitectureScores DefaultScores() => new(7, 8, 6, 7);

    [Fact]
    public void Entity_SameId_ShouldBeEqual()
    {
        var id = Guid.NewGuid();
        var a = AnalysisReport.Reconstitute(id, Guid.NewGuid(), Guid.NewGuid(),
            [], [], [], [], DefaultScores(), 7.0, 0.9, [], 100, DateTime.UtcNow);
        var b = AnalysisReport.Reconstitute(id, Guid.NewGuid(), Guid.NewGuid(),
            [], [], [], [], DefaultScores(), 5.0, 0.5, [], 200, DateTime.UtcNow);

        a.Equals(b).Should().BeTrue();
        a.Equals((object)b).Should().BeTrue();
    }

    [Fact]
    public void Entity_DifferentId_ShouldNotBeEqual()
    {
        var a = AnalysisReport.Reconstitute(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            [], [], [], [], DefaultScores(), 7.0, 0.9, [], 100, DateTime.UtcNow);
        var b = AnalysisReport.Reconstitute(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            [], [], [], [], DefaultScores(), 7.0, 0.9, [], 100, DateTime.UtcNow);

        a.Equals(b).Should().BeFalse();
    }

    [Fact]
    public void Entity_EqualsNull_ShouldReturnFalse()
    {
        var report = AnalysisReport.Create(Guid.NewGuid(), Guid.NewGuid(),
            [], [], [], [], DefaultScores(), 0.5, [], 100);

        report.Equals((AnalysisReport?)null).Should().BeFalse();
        report.Equals((object?)null).Should().BeFalse();
    }

    [Fact]
    public void Entity_GetHashCode_SameId_ShouldBeEqual()
    {
        var id = Guid.NewGuid();
        var a = AnalysisReport.Reconstitute(id, Guid.NewGuid(), Guid.NewGuid(),
            [], [], [], [], DefaultScores(), 7.0, 0.9, [], 100, DateTime.UtcNow);
        var b = AnalysisReport.Reconstitute(id, Guid.NewGuid(), Guid.NewGuid(),
            [], [], [], [], DefaultScores(), 5.0, 0.5, [], 200, DateTime.UtcNow);

        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void Entity_OperatorEquals_ShouldWork()
    {
        var id = Guid.NewGuid();
        var a = AnalysisReport.Reconstitute(id, Guid.NewGuid(), Guid.NewGuid(),
            [], [], [], [], DefaultScores(), 7.0, 0.9, [], 100, DateTime.UtcNow);
        var b = AnalysisReport.Reconstitute(id, Guid.NewGuid(), Guid.NewGuid(),
            [], [], [], [], DefaultScores(), 5.0, 0.5, [], 200, DateTime.UtcNow);

        (a == b).Should().BeTrue();
        (a != b).Should().BeFalse();
    }

    [Fact]
    public void Entity_OperatorNotEquals_DifferentIds_ShouldReturnTrue()
    {
        var a = AnalysisReport.Create(Guid.NewGuid(), Guid.NewGuid(),
            [], [], [], [], DefaultScores(), 0.5, [], 100);
        var b = AnalysisReport.Create(Guid.NewGuid(), Guid.NewGuid(),
            [], [], [], [], DefaultScores(), 0.5, [], 100);

        (a != b).Should().BeTrue();
    }

    [Fact]
    public void Entity_EqualsNonEntityObject_ShouldReturnFalse()
    {
        var report = AnalysisReport.Create(Guid.NewGuid(), Guid.NewGuid(),
            [], [], [], [], DefaultScores(), 0.5, [], 100);

        report.Equals("not an entity").Should().BeFalse();
    }

    [Fact]
    public void DomainEvents_ShouldBeEmpty_AfterCreate()
    {
        var report = AnalysisReport.Create(Guid.NewGuid(), Guid.NewGuid(),
            [], [], [], [], DefaultScores(), 0.5, [], 100);

        report.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void PopDomainEvents_ShouldReturnAndClear()
    {
        var report = AnalysisReport.Create(Guid.NewGuid(), Guid.NewGuid(),
            [], [], [], [], DefaultScores(), 0.5, [], 100);

        var events = report.PopDomainEvents();
        events.Should().BeEmpty();
    }
}

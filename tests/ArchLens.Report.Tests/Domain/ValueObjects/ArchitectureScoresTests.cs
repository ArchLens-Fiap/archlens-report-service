using ArchLens.Report.Domain.ValueObjects.Reports;
using FluentAssertions;

namespace ArchLens.Report.Tests.Domain.ValueObjects;

public class ArchitectureScoresTests
{
    [Fact]
    public void ShouldClamp_Values()
    {
        var scores = new ArchitectureScores(-1, 15, 5, 7);

        scores.Scalability.Should().Be(0);
        scores.Security.Should().Be(10);
        scores.Reliability.Should().Be(5);
        scores.Maintainability.Should().Be(7);
    }

    [Fact]
    public void Overall_ShouldBeAverage()
    {
        var scores = new ArchitectureScores(8, 6, 7, 7);

        scores.Overall.Should().Be(7.0);
    }

    [Fact]
    public void Equality_ShouldWorkCorrectly()
    {
        var a = new ArchitectureScores(7, 8, 6, 7);
        var b = new ArchitectureScores(7, 8, 6, 7);

        a.Should().Be(b);
    }
}

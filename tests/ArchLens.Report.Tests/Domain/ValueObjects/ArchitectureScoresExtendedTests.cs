using ArchLens.Report.Domain.ValueObjects.Reports;
using FluentAssertions;

namespace ArchLens.Report.Tests.Domain.ValueObjects;

public class ArchitectureScoresExtendedTests
{
    [Fact]
    public void Constructor_AllZeros_ShouldHaveZeroOverall()
    {
        var scores = new ArchitectureScores(0, 0, 0, 0);

        scores.Overall.Should().Be(0);
    }

    [Fact]
    public void Constructor_AllTen_ShouldHaveTenOverall()
    {
        var scores = new ArchitectureScores(10, 10, 10, 10);

        scores.Overall.Should().Be(10);
    }

    [Fact]
    public void Equality_DifferentValues_ShouldNotBeEqual()
    {
        var a = new ArchitectureScores(7, 8, 6, 7);
        var b = new ArchitectureScores(7, 8, 6, 9);

        a.Should().NotBe(b);
    }

    [Fact]
    public void Constructor_NegativeValues_ShouldClampToZero()
    {
        var scores = new ArchitectureScores(-5, -10, -1, -100);

        scores.Scalability.Should().Be(0);
        scores.Security.Should().Be(0);
        scores.Reliability.Should().Be(0);
        scores.Maintainability.Should().Be(0);
    }

    [Fact]
    public void Constructor_OverTen_ShouldClampToTen()
    {
        var scores = new ArchitectureScores(15, 20, 100, 50);

        scores.Scalability.Should().Be(10);
        scores.Security.Should().Be(10);
        scores.Reliability.Should().Be(10);
        scores.Maintainability.Should().Be(10);
    }

    [Fact]
    public void Overall_ShouldComputeCorrectly()
    {
        var scores = new ArchitectureScores(4, 6, 8, 10);

        scores.Overall.Should().Be(7.0);
    }
}

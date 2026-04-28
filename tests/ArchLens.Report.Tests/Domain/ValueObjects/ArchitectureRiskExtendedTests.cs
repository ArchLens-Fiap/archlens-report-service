using ArchLens.Report.Domain.ValueObjects.Reports;
using FluentAssertions;

namespace ArchLens.Report.Tests.Domain.ValueObjects;

public class ArchitectureRiskExtendedTests
{
    [Fact]
    public void Constructor_ValidData_ShouldSetAllProperties()
    {
        var risk = new ArchitectureRisk("SPOF", "Single point of failure", "high", "reliability", "Add replica");

        risk.Title.Should().Be("SPOF");
        risk.Description.Should().Be("Single point of failure");
        risk.Severity.Should().Be("high");
        risk.Category.Should().Be("reliability");
        risk.Mitigation.Should().Be("Add replica");
    }

    [Fact]
    public void WithNullDescription_ShouldThrow()
    {
        var act = () => new ArchitectureRisk("Title", null!, "high", "security", "fix");

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WithEmptySeverity_ShouldThrow()
    {
        var act = () => new ArchitectureRisk("Title", "desc", "", "security", "fix");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void WithEmptyCategory_ShouldThrow()
    {
        var act = () => new ArchitectureRisk("Title", "desc", "high", "", "fix");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void WithNullMitigation_ShouldThrow()
    {
        var act = () => new ArchitectureRisk("Title", "desc", "high", "security", null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Equality_SameTitle_DifferentDescription_ShouldBeEqual()
    {
        var a = new ArchitectureRisk("SPOF", "desc1", "high", "reliability", "fix1");
        var b = new ArchitectureRisk("SPOF", "desc2", "high", "performance", "fix2");

        a.Should().Be(b);
    }

    [Fact]
    public void Equality_DifferentTitle_ShouldNotBeEqual()
    {
        var a = new ArchitectureRisk("SPOF", "desc", "high", "reliability", "fix");
        var b = new ArchitectureRisk("Other", "desc", "high", "reliability", "fix");

        a.Should().NotBe(b);
    }

    [Fact]
    public void Equality_DifferentSeverity_ShouldNotBeEqual()
    {
        var a = new ArchitectureRisk("SPOF", "desc", "high", "reliability", "fix");
        var b = new ArchitectureRisk("SPOF", "desc", "low", "reliability", "fix");

        a.Should().NotBe(b);
    }

    [Fact]
    public void WithWhitespaceTitle_ShouldThrow()
    {
        var act = () => new ArchitectureRisk("   ", "desc", "high", "security", "fix");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void WithEmptyDescriptionString_ShouldSucceed()
    {
        var risk = new ArchitectureRisk("Title", "", "high", "security", "fix");

        risk.Description.Should().BeEmpty();
    }

    [Fact]
    public void WithEmptyMitigationString_ShouldSucceed()
    {
        var risk = new ArchitectureRisk("Title", "desc", "high", "security", "");

        risk.Mitigation.Should().BeEmpty();
    }
}

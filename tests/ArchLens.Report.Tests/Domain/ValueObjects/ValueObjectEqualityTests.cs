using ArchLens.Report.Domain.ValueObjects.Reports;
using FluentAssertions;

namespace ArchLens.Report.Tests.Domain.ValueObjects;

public class ValueObjectEqualityTests
{
    [Fact]
    public void ValueObject_EqualsNull_ShouldReturnFalse()
    {
        var scores = new ArchitectureScores(7, 8, 6, 7);

        scores.Equals(null).Should().BeFalse();
    }

    [Fact]
    public void ValueObject_EqualsSelf_ShouldReturnTrue()
    {
        var scores = new ArchitectureScores(7, 8, 6, 7);

        scores.Equals(scores).Should().BeTrue();
    }

    [Fact]
    public void ValueObject_EqualsObjectNull_ShouldReturnFalse()
    {
        var scores = new ArchitectureScores(7, 8, 6, 7);

        scores.Equals((object?)null).Should().BeFalse();
    }

    [Fact]
    public void ValueObject_EqualsDifferentType_ShouldReturnFalse()
    {
        var scores = new ArchitectureScores(7, 8, 6, 7);

        scores.Equals("not a value object").Should().BeFalse();
    }

    [Fact]
    public void ValueObject_OperatorEquals_SameValues_ShouldReturnTrue()
    {
        var a = new ArchitectureScores(7, 8, 6, 7);
        var b = new ArchitectureScores(7, 8, 6, 7);

        (a == b).Should().BeTrue();
    }

    [Fact]
    public void ValueObject_OperatorNotEquals_DifferentValues_ShouldReturnTrue()
    {
        var a = new ArchitectureScores(7, 8, 6, 7);
        var b = new ArchitectureScores(9, 8, 6, 7);

        (a != b).Should().BeTrue();
    }

    [Fact]
    public void ValueObject_OperatorEquals_BothNull_ShouldReturnTrue()
    {
        ArchitectureScores? a = null;
        ArchitectureScores? b = null;

        (a == b).Should().BeTrue();
    }

    [Fact]
    public void ValueObject_OperatorEquals_OneNull_ShouldReturnFalse()
    {
        var a = new ArchitectureScores(7, 8, 6, 7);
        ArchitectureScores? b = null;

        (a == b).Should().BeFalse();
        (b == a).Should().BeFalse();
    }

    [Fact]
    public void ValueObject_GetHashCode_EqualObjects_ShouldBeEqual()
    {
        var a = new ArchitectureScores(7, 8, 6, 7);
        var b = new ArchitectureScores(7, 8, 6, 7);

        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void ValueObject_EqualsOtherVO_DifferentType_ShouldReturnFalse()
    {
        var scores = new ArchitectureScores(7, 8, 6, 7);
        var risk = new ArchitectureRisk("Title", "desc", "high", "security", "fix");

        scores.Equals((object)risk).Should().BeFalse();
    }

    [Fact]
    public void IdentifiedComponent_GetHashCode_SameNameType_ShouldBeEqual()
    {
        var a = new IdentifiedComponent("GW", "gateway", "desc1", 0.9);
        var b = new IdentifiedComponent("GW", "gateway", "desc2", 0.5);

        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void IdentifiedConnection_OperatorEquals_ShouldWork()
    {
        var a = new IdentifiedConnection("A", "B", "HTTP", "desc1");
        var b = new IdentifiedConnection("A", "B", "HTTP", "desc2");

        (a == b).Should().BeTrue();
    }

    [Fact]
    public void IdentifiedConnection_OperatorNotEquals_ShouldWork()
    {
        var a = new IdentifiedConnection("A", "B", "HTTP", "desc");
        var b = new IdentifiedConnection("A", "C", "HTTP", "desc");

        (a != b).Should().BeTrue();
    }

    [Fact]
    public void ArchitectureRisk_GetHashCode_ShouldBeConsistent()
    {
        var risk = new ArchitectureRisk("SPOF", "desc", "high", "reliability", "fix");

        var hash1 = risk.GetHashCode();
        var hash2 = risk.GetHashCode();

        hash1.Should().Be(hash2);
    }
}

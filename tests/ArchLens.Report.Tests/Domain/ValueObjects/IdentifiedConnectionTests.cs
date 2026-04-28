using ArchLens.Report.Domain.ValueObjects.Reports;
using FluentAssertions;

namespace ArchLens.Report.Tests.Domain.ValueObjects;

public class IdentifiedConnectionTests
{
    [Fact]
    public void Equality_BySourceTargetType()
    {
        var a = new IdentifiedConnection("A", "B", "HTTP", "desc1");
        var b = new IdentifiedConnection("A", "B", "HTTP", "different desc");

        a.Should().Be(b);
    }

    [Fact]
    public void WithEmptySource_ShouldThrow()
    {
        var act = () => new IdentifiedConnection("", "B", "HTTP", "desc");

        act.Should().Throw<ArgumentException>();
    }
}

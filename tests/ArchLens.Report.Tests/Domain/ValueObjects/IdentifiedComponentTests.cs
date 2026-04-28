using ArchLens.Report.Domain.ValueObjects.Reports;
using FluentAssertions;

namespace ArchLens.Report.Tests.Domain.ValueObjects;

public class IdentifiedComponentTests
{
    [Fact]
    public void ShouldClamp_Confidence()
    {
        var comp = new IdentifiedComponent("Test", "service", "desc", 1.5);

        comp.Confidence.Should().Be(1.0);
    }

    [Fact]
    public void WithEmptyName_ShouldThrow()
    {
        var act = () => new IdentifiedComponent("", "service", "desc", 0.5);

        act.Should().Throw<ArgumentException>();
    }
}

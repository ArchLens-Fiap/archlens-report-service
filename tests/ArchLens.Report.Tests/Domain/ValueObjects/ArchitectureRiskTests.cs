using ArchLens.Report.Domain.ValueObjects.Reports;
using FluentAssertions;

namespace ArchLens.Report.Tests.Domain.ValueObjects;

public class ArchitectureRiskTests
{
    [Fact]
    public void WithEmptyTitle_ShouldThrow()
    {
        var act = () => new ArchitectureRisk("", "desc", "high", "security", "fix");

        act.Should().Throw<ArgumentException>();
    }
}

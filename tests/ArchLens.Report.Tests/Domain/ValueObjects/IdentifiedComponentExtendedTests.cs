using ArchLens.Report.Domain.ValueObjects.Reports;
using FluentAssertions;

namespace ArchLens.Report.Tests.Domain.ValueObjects;

public class IdentifiedComponentExtendedTests
{
    [Fact]
    public void Constructor_ValidData_ShouldSetAllProperties()
    {
        var comp = new IdentifiedComponent("ServiceA", "microservice", "Handles A", 0.85);

        comp.Name.Should().Be("ServiceA");
        comp.Type.Should().Be("microservice");
        comp.Description.Should().Be("Handles A");
        comp.Confidence.Should().Be(0.85);
    }

    [Fact]
    public void WithEmptyType_ShouldThrow()
    {
        var act = () => new IdentifiedComponent("Name", "", "desc", 0.5);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void WithNullDescription_ShouldThrow()
    {
        var act = () => new IdentifiedComponent("Name", "type", null!, 0.5);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void NegativeConfidence_ShouldClampToZero()
    {
        var comp = new IdentifiedComponent("Name", "type", "desc", -0.5);

        comp.Confidence.Should().Be(0.0);
    }

    [Fact]
    public void Equality_SameNameAndType_ShouldBeEqual()
    {
        var a = new IdentifiedComponent("GW", "gateway", "desc1", 0.9);
        var b = new IdentifiedComponent("GW", "gateway", "desc2", 0.5);

        a.Should().Be(b);
    }

    [Fact]
    public void Equality_DifferentName_ShouldNotBeEqual()
    {
        var a = new IdentifiedComponent("GW1", "gateway", "desc", 0.9);
        var b = new IdentifiedComponent("GW2", "gateway", "desc", 0.9);

        a.Should().NotBe(b);
    }

    [Fact]
    public void Equality_DifferentType_ShouldNotBeEqual()
    {
        var a = new IdentifiedComponent("GW", "gateway", "desc", 0.9);
        var b = new IdentifiedComponent("GW", "microservice", "desc", 0.9);

        a.Should().NotBe(b);
    }

    [Fact]
    public void WithWhitespaceName_ShouldThrow()
    {
        var act = () => new IdentifiedComponent("   ", "type", "desc", 0.5);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void WithNullName_ShouldThrow()
    {
        var act = () => new IdentifiedComponent(null!, "type", "desc", 0.5);

        act.Should().Throw<ArgumentException>();
    }
}

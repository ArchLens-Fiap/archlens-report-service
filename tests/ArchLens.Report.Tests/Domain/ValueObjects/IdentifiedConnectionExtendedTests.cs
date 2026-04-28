using ArchLens.Report.Domain.ValueObjects.Reports;
using FluentAssertions;

namespace ArchLens.Report.Tests.Domain.ValueObjects;

public class IdentifiedConnectionExtendedTests
{
    [Fact]
    public void Constructor_ValidData_ShouldSetAllProperties()
    {
        var conn = new IdentifiedConnection("A", "B", "HTTP", "REST call");

        conn.Source.Should().Be("A");
        conn.Target.Should().Be("B");
        conn.Type.Should().Be("HTTP");
        conn.Description.Should().Be("REST call");
    }

    [Fact]
    public void WithEmptyTarget_ShouldThrow()
    {
        var act = () => new IdentifiedConnection("A", "", "HTTP", "desc");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void WithNullType_ShouldThrow()
    {
        var act = () => new IdentifiedConnection("A", "B", null!, "desc");

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WithNullDescription_ShouldThrow()
    {
        var act = () => new IdentifiedConnection("A", "B", "HTTP", null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Equality_DifferentSource_ShouldNotBeEqual()
    {
        var a = new IdentifiedConnection("A", "B", "HTTP", "desc");
        var b = new IdentifiedConnection("C", "B", "HTTP", "desc");

        a.Should().NotBe(b);
    }

    [Fact]
    public void Equality_DifferentTarget_ShouldNotBeEqual()
    {
        var a = new IdentifiedConnection("A", "B", "HTTP", "desc");
        var b = new IdentifiedConnection("A", "C", "HTTP", "desc");

        a.Should().NotBe(b);
    }

    [Fact]
    public void Equality_DifferentType_ShouldNotBeEqual()
    {
        var a = new IdentifiedConnection("A", "B", "HTTP", "desc");
        var b = new IdentifiedConnection("A", "B", "gRPC", "desc");

        a.Should().NotBe(b);
    }

    [Fact]
    public void WithWhitespaceSource_ShouldThrow()
    {
        var act = () => new IdentifiedConnection("   ", "B", "HTTP", "desc");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void WithNullSource_ShouldThrow()
    {
        var act = () => new IdentifiedConnection(null!, "B", "HTTP", "desc");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void WithEmptyTypeString_ShouldSucceed()
    {
        var conn = new IdentifiedConnection("A", "B", "", "desc");

        conn.Type.Should().BeEmpty();
    }
}

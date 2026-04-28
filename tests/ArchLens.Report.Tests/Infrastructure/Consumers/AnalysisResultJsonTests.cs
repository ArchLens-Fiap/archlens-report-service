using System.Text.Json;
using ArchLens.Report.Infrastructure.Consumers;
using FluentAssertions;

namespace ArchLens.Report.Tests.Infrastructure.Consumers;

public class AnalysisResultJsonTests
{
    [Fact]
    public void AnalysisResultJson_DefaultValues_ShouldBeNull()
    {
        var result = new AnalysisResultJson();

        result.Components.Should().BeNull();
        result.Connections.Should().BeNull();
        result.Risks.Should().BeNull();
        result.Recommendations.Should().BeNull();
        result.Scores.Should().BeNull();
        result.Confidence.Should().Be(0);
    }

    [Fact]
    public void AnalysisResultJson_WithInit_ShouldStoreValues()
    {
        var result = new AnalysisResultJson
        {
            Components = [new ComponentJson { Name = "Svc", Type = "api", Description = "desc", Confidence = 0.9 }],
            Connections = [new ConnectionJson { Source = "A", Target = "B", Type = "HTTP", Description = "call" }],
            Risks = [new RiskJson { Title = "R1", Description = "d", Severity = "high", Category = "sec", Mitigation = "fix" }],
            Recommendations = ["Cache"],
            Scores = new ScoresJson { Scalability = 7, Security = 8, Reliability = 6, Maintainability = 7 },
            Confidence = 0.85
        };

        result.Components.Should().HaveCount(1);
        result.Connections.Should().HaveCount(1);
        result.Risks.Should().HaveCount(1);
        result.Recommendations.Should().ContainSingle();
        result.Scores.Should().NotBeNull();
        result.Confidence.Should().Be(0.85);
    }

    [Fact]
    public void ComponentJson_ShouldStoreAllProperties()
    {
        var comp = new ComponentJson
        {
            Name = "Gateway",
            Type = "gateway",
            Description = "API Gateway",
            Confidence = 0.95
        };

        comp.Name.Should().Be("Gateway");
        comp.Type.Should().Be("gateway");
        comp.Description.Should().Be("API Gateway");
        comp.Confidence.Should().Be(0.95);
    }

    [Fact]
    public void ComponentJson_DefaultValues_ShouldBeNullAndZero()
    {
        var comp = new ComponentJson();

        comp.Name.Should().BeNull();
        comp.Type.Should().BeNull();
        comp.Description.Should().BeNull();
        comp.Confidence.Should().Be(0);
    }

    [Fact]
    public void ConnectionJson_ShouldStoreAllProperties()
    {
        var conn = new ConnectionJson
        {
            Source = "ServiceA",
            Target = "ServiceB",
            Type = "gRPC",
            Description = "Internal comm"
        };

        conn.Source.Should().Be("ServiceA");
        conn.Target.Should().Be("ServiceB");
        conn.Type.Should().Be("gRPC");
        conn.Description.Should().Be("Internal comm");
    }

    [Fact]
    public void ConnectionJson_DefaultValues_ShouldBeNull()
    {
        var conn = new ConnectionJson();

        conn.Source.Should().BeNull();
        conn.Target.Should().BeNull();
        conn.Type.Should().BeNull();
        conn.Description.Should().BeNull();
    }

    [Fact]
    public void RiskJson_ShouldStoreAllProperties()
    {
        var risk = new RiskJson
        {
            Title = "SPOF",
            Description = "Single point",
            Severity = "critical",
            Category = "reliability",
            Mitigation = "Add replica"
        };

        risk.Title.Should().Be("SPOF");
        risk.Description.Should().Be("Single point");
        risk.Severity.Should().Be("critical");
        risk.Category.Should().Be("reliability");
        risk.Mitigation.Should().Be("Add replica");
    }

    [Fact]
    public void RiskJson_DefaultValues_ShouldBeNull()
    {
        var risk = new RiskJson();

        risk.Title.Should().BeNull();
        risk.Description.Should().BeNull();
        risk.Severity.Should().BeNull();
        risk.Category.Should().BeNull();
        risk.Mitigation.Should().BeNull();
    }

    [Fact]
    public void ScoresJson_ShouldStoreAllProperties()
    {
        var scores = new ScoresJson
        {
            Scalability = 8.5,
            Security = 7.0,
            Reliability = 9.0,
            Maintainability = 6.5
        };

        scores.Scalability.Should().Be(8.5);
        scores.Security.Should().Be(7.0);
        scores.Reliability.Should().Be(9.0);
        scores.Maintainability.Should().Be(6.5);
    }

    [Fact]
    public void ScoresJson_DefaultValues_ShouldBeZero()
    {
        var scores = new ScoresJson();

        scores.Scalability.Should().Be(0);
        scores.Security.Should().Be(0);
        scores.Reliability.Should().Be(0);
        scores.Maintainability.Should().Be(0);
    }

    [Fact]
    public void AnalysisResultJson_Deserialization_ShouldWorkWithCaseInsensitive()
    {
        var json = """
        {
            "components": [{"name": "Svc", "type": "api", "description": "desc", "confidence": 0.9}],
            "connections": [{"source": "A", "target": "B", "type": "HTTP", "description": "call"}],
            "risks": [{"title": "R", "description": "d", "severity": "high", "category": "sec", "mitigation": "fix"}],
            "recommendations": ["Cache"],
            "scores": {"scalability": 7, "security": 8, "reliability": 6, "maintainability": 7},
            "confidence": 0.85
        }
        """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<AnalysisResultJson>(json, options);

        result.Should().NotBeNull();
        result!.Components.Should().HaveCount(1);
        result.Components![0].Name.Should().Be("Svc");
        result.Connections.Should().HaveCount(1);
        result.Connections![0].Source.Should().Be("A");
        result.Risks.Should().HaveCount(1);
        result.Risks![0].Title.Should().Be("R");
        result.Recommendations.Should().ContainSingle().Which.Should().Be("Cache");
        result.Scores!.Scalability.Should().Be(7);
        result.Confidence.Should().Be(0.85);
    }

    [Fact]
    public void AnalysisResultJson_Deserialization_WithNullFields_ShouldProduceNulls()
    {
        var json = """
        {
            "confidence": 0.5
        }
        """;

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<AnalysisResultJson>(json, options);

        result.Should().NotBeNull();
        result!.Components.Should().BeNull();
        result.Connections.Should().BeNull();
        result.Risks.Should().BeNull();
        result.Recommendations.Should().BeNull();
        result.Scores.Should().BeNull();
        result.Confidence.Should().Be(0.5);
    }

    [Fact]
    public void AnalysisResultJson_RecordEquality_ShouldWorkCorrectly()
    {
        var a = new AnalysisResultJson { Confidence = 0.85 };
        var b = new AnalysisResultJson { Confidence = 0.85 };
        var c = new AnalysisResultJson { Confidence = 0.5 };

        a.Should().Be(b);
        a.Should().NotBe(c);
    }

    [Fact]
    public void ComponentJson_RecordEquality_ShouldWorkCorrectly()
    {
        var a = new ComponentJson { Name = "Svc", Type = "api", Description = "d", Confidence = 0.9 };
        var b = new ComponentJson { Name = "Svc", Type = "api", Description = "d", Confidence = 0.9 };

        a.Should().Be(b);
    }

    [Fact]
    public void ConnectionJson_RecordEquality_ShouldWorkCorrectly()
    {
        var a = new ConnectionJson { Source = "A", Target = "B", Type = "HTTP", Description = "c" };
        var b = new ConnectionJson { Source = "A", Target = "B", Type = "HTTP", Description = "c" };

        a.Should().Be(b);
    }

    [Fact]
    public void RiskJson_RecordEquality_ShouldWorkCorrectly()
    {
        var a = new RiskJson { Title = "R", Description = "d", Severity = "h", Category = "s", Mitigation = "f" };
        var b = new RiskJson { Title = "R", Description = "d", Severity = "h", Category = "s", Mitigation = "f" };

        a.Should().Be(b);
    }

    [Fact]
    public void ScoresJson_RecordEquality_ShouldWorkCorrectly()
    {
        var a = new ScoresJson { Scalability = 7, Security = 8, Reliability = 6, Maintainability = 7 };
        var b = new ScoresJson { Scalability = 7, Security = 8, Reliability = 6, Maintainability = 7 };

        a.Should().Be(b);
    }
}

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ArchLens.Report.Infrastructure.Persistence.MongoDB.Documents;

public sealed class ReportDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    public Guid AnalysisId { get; set; }
    public Guid DiagramId { get; set; }
    public string? UserId { get; set; }

    public List<ComponentDoc> Components { get; set; } = [];
    public List<ConnectionDoc> Connections { get; set; } = [];
    public List<RiskDoc> Risks { get; set; } = [];
    public List<string> Recommendations { get; set; } = [];

    public ScoresDoc Scores { get; set; } = null!;
    public double OverallScore { get; set; }
    public double Confidence { get; set; }

    public List<string> ProvidersUsed { get; set; } = [];
    public long ProcessingTimeMs { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class ComponentDoc
{
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Description { get; set; } = null!;
    public double Confidence { get; set; }
}

public sealed class ConnectionDoc
{
    public string Source { get; set; } = null!;
    public string Target { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Description { get; set; } = null!;
}

public sealed class RiskDoc
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Severity { get; set; } = null!;
    public string Category { get; set; } = null!;
    public string Mitigation { get; set; } = null!;
}

public sealed class ScoresDoc
{
    public double Scalability { get; set; }
    public double Security { get; set; }
    public double Reliability { get; set; }
    public double Maintainability { get; set; }
}

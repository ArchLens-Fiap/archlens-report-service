namespace ArchLens.Report.Infrastructure.Consumers;

public sealed record AnalysisResultJson
{
    public List<ComponentJson>? Components { get; init; }
    public List<ConnectionJson>? Connections { get; init; }
    public List<RiskJson>? Risks { get; init; }
    public List<string>? Recommendations { get; init; }
    public ScoresJson? Scores { get; init; }
    public double Confidence { get; init; }
}

public sealed record ComponentJson
{
    public string? Name { get; init; }
    public string? Type { get; init; }
    public string? Description { get; init; }
    public double Confidence { get; init; }
}

public sealed record ConnectionJson
{
    public string? Source { get; init; }
    public string? Target { get; init; }
    public string? Type { get; init; }
    public string? Description { get; init; }
}

public sealed record RiskJson
{
    public string? Title { get; init; }
    public string? Description { get; init; }
    public string? Severity { get; init; }
    public string? Category { get; init; }
    public string? Mitigation { get; init; }
}

public sealed record ScoresJson
{
    public double Scalability { get; init; }
    public double Security { get; init; }
    public double Reliability { get; init; }
    public double Maintainability { get; init; }
}

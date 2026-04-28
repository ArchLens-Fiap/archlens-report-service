using ArchLens.Report.Domain.ValueObjects.Reports;
using ArchLens.SharedKernel.Domain;

namespace ArchLens.Report.Domain.Entities.ReportEntities;

public sealed class AnalysisReport : AggregateRoot<Guid>
{
    public Guid AnalysisId { get; private set; }
    public Guid DiagramId { get; private set; }
    public string? UserId { get; private set; }
    public IReadOnlyList<IdentifiedComponent> Components { get; private set; } = [];
    public IReadOnlyList<IdentifiedConnection> Connections { get; private set; } = [];
    public IReadOnlyList<ArchitectureRisk> Risks { get; private set; } = [];
    public IReadOnlyList<string> Recommendations { get; private set; } = [];
    public ArchitectureScores Scores { get; private set; } = null!;
    public double OverallScore { get; private set; }
    public double Confidence { get; private set; }
    public IReadOnlyList<string> ProvidersUsed { get; private set; } = [];
    public long ProcessingTimeMs { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private AnalysisReport() { }
    private AnalysisReport(Guid id) : base(id) { }

    public static AnalysisReport Create(
        Guid analysisId,
        Guid diagramId,
        IReadOnlyList<IdentifiedComponent> components,
        IReadOnlyList<IdentifiedConnection> connections,
        IReadOnlyList<ArchitectureRisk> risks,
        IReadOnlyList<string> recommendations,
        ArchitectureScores scores,
        double confidence,
        IReadOnlyList<string> providersUsed,
        long processingTimeMs,
        string? userId = null)
    {
        ArgumentNullException.ThrowIfNull(components);
        ArgumentNullException.ThrowIfNull(connections);
        ArgumentNullException.ThrowIfNull(risks);
        ArgumentNullException.ThrowIfNull(recommendations);
        ArgumentNullException.ThrowIfNull(scores);
        ArgumentNullException.ThrowIfNull(providersUsed);

        if (analysisId == Guid.Empty) throw new ArgumentException("AnalysisId cannot be empty.", nameof(analysisId));
        if (diagramId == Guid.Empty) throw new ArgumentException("DiagramId cannot be empty.", nameof(diagramId));

        return new AnalysisReport(Guid.NewGuid())
        {
            AnalysisId = analysisId,
            DiagramId = diagramId,
            Components = components,
            Connections = connections,
            Risks = risks,
            Recommendations = recommendations,
            Scores = scores,
            OverallScore = scores.Overall,
            Confidence = Math.Clamp(confidence, 0, 1),
            ProvidersUsed = providersUsed,
            ProcessingTimeMs = processingTimeMs,
            CreatedAt = DateTime.UtcNow,
            UserId = userId
        };
    }

    public static AnalysisReport Reconstitute(
        Guid id,
        Guid analysisId,
        Guid diagramId,
        IReadOnlyList<IdentifiedComponent> components,
        IReadOnlyList<IdentifiedConnection> connections,
        IReadOnlyList<ArchitectureRisk> risks,
        IReadOnlyList<string> recommendations,
        ArchitectureScores scores,
        double overallScore,
        double confidence,
        IReadOnlyList<string> providersUsed,
        long processingTimeMs,
        DateTime createdAt,
        string? userId = null)
    {
        return new AnalysisReport(id)
        {
            AnalysisId = analysisId,
            DiagramId = diagramId,
            Components = components,
            Connections = connections,
            Risks = risks,
            Recommendations = recommendations,
            Scores = scores,
            OverallScore = overallScore,
            Confidence = confidence,
            ProvidersUsed = providersUsed,
            ProcessingTimeMs = processingTimeMs,
            CreatedAt = createdAt,
            UserId = userId
        };
    }
}

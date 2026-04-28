using ArchLens.SharedKernel.Domain;

namespace ArchLens.Report.Domain.ValueObjects.Reports;

public sealed class ArchitectureRisk : ValueObject
{
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public string Severity { get; private set; } = null!;
    public string Category { get; private set; } = null!;
    public string Mitigation { get; private set; } = null!;

    private ArchitectureRisk() { }

    public ArchitectureRisk(string title, string description, string severity, string category, string mitigation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentNullException.ThrowIfNull(description);
        ArgumentException.ThrowIfNullOrWhiteSpace(severity);
        ArgumentException.ThrowIfNullOrWhiteSpace(category);
        ArgumentNullException.ThrowIfNull(mitigation);

        Title = title;
        Description = description;
        Severity = severity;
        Category = category;
        Mitigation = mitigation;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Title;
        yield return Severity;
    }
}

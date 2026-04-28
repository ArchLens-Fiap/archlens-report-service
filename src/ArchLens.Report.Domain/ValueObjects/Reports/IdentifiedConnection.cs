using ArchLens.SharedKernel.Domain;

namespace ArchLens.Report.Domain.ValueObjects.Reports;

public sealed class IdentifiedConnection : ValueObject
{
    public string Source { get; private set; } = null!;
    public string Target { get; private set; } = null!;
    public string Type { get; private set; } = null!;
    public string Description { get; private set; } = null!;

    private IdentifiedConnection() { }

    public IdentifiedConnection(string source, string target, string type, string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(source);
        ArgumentException.ThrowIfNullOrWhiteSpace(target);
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(description);

        Source = source;
        Target = target;
        Type = type;
        Description = description;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Source;
        yield return Target;
        yield return Type;
    }
}

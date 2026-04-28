using ArchLens.SharedKernel.Domain;

namespace ArchLens.Report.Domain.ValueObjects.Reports;

public sealed class IdentifiedComponent : ValueObject
{
    public string Name { get; private set; } = null!;
    public string Type { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public double Confidence { get; private set; }

    private IdentifiedComponent() { }

    public IdentifiedComponent(string name, string type, string description, double confidence)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(type);
        ArgumentNullException.ThrowIfNull(description);

        Name = name;
        Type = type;
        Description = description;
        Confidence = Math.Clamp(confidence, 0, 1);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return Type;
    }
}

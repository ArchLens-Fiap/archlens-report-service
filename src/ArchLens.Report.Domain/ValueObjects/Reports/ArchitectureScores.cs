using ArchLens.SharedKernel.Domain;

namespace ArchLens.Report.Domain.ValueObjects.Reports;

public sealed class ArchitectureScores : ValueObject
{
    public double Scalability { get; private set; }
    public double Security { get; private set; }
    public double Reliability { get; private set; }
    public double Maintainability { get; private set; }

    private ArchitectureScores() { }

    public ArchitectureScores(double scalability, double security, double reliability, double maintainability)
    {
        Scalability = Math.Clamp(scalability, 0, 10);
        Security = Math.Clamp(security, 0, 10);
        Reliability = Math.Clamp(reliability, 0, 10);
        Maintainability = Math.Clamp(maintainability, 0, 10);
    }

    public double Overall => (Scalability + Security + Reliability + Maintainability) / 4.0;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Scalability;
        yield return Security;
        yield return Reliability;
        yield return Maintainability;
    }
}

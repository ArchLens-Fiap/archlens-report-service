using ArchLens.Report.Application.UseCases.Reports.Queries.GetByAnalysis;
using ArchLens.Report.Domain.Entities.ReportEntities;
using ArchLens.Report.Domain.Interfaces.ReportInterfaces;
using ArchLens.Report.Domain.ValueObjects.Reports;
using ArchLens.SharedKernel.Application;
using FluentAssertions;
using NSubstitute;

namespace ArchLens.Report.Tests.Application.UseCases.Reports;

public class GetByAnalysisHandlerTests
{
    private readonly IReportRepository _repository = Substitute.For<IReportRepository>();
    private readonly GetReportByAnalysisHandler _handler;

    public GetByAnalysisHandlerTests()
    {
        _handler = new GetReportByAnalysisHandler(_repository);
    }

    private static AnalysisReport CreateReport()
    {
        return AnalysisReport.Create(
            Guid.NewGuid(), Guid.NewGuid(),
            [new IdentifiedComponent("GW", "gateway", "Gateway", 0.9)],
            [new IdentifiedConnection("A", "B", "HTTP", "call")],
            [new ArchitectureRisk("Risk", "desc", "high", "security", "fix")],
            ["Add cache"],
            new ArchitectureScores(7, 8, 6, 7),
            0.85, ["openai"], 1200);
    }

    [Fact]
    public async Task Handle_NotFound_ShouldReturnFailure()
    {
        _repository.GetByAnalysisIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((AnalysisReport?)null);

        var result = await _handler.Handle(new GetReportByAnalysisQuery(Guid.NewGuid()), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.NotFound);
    }

    [Fact]
    public async Task Handle_Found_ShouldMapCorrectly()
    {
        var report = CreateReport();
        _repository.GetByAnalysisIdAsync(report.AnalysisId, Arg.Any<CancellationToken>())
            .Returns(report);

        var result = await _handler.Handle(new GetReportByAnalysisQuery(report.AnalysisId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(report.Id);
        result.Value.AnalysisId.Should().Be(report.AnalysisId);
        result.Value.Components.Should().HaveCount(1);
        result.Value.Connections.Should().HaveCount(1);
        result.Value.Risks.Should().HaveCount(1);
        result.Value.OverallScore.Should().Be(report.OverallScore);
    }
}

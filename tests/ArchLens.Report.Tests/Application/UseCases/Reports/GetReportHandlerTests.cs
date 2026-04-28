using ArchLens.Report.Application.UseCases.Reports.Queries.GetById;
using ArchLens.Report.Application.UseCases.Reports.Queries.GetByAnalysis;
using ArchLens.Report.Application.UseCases.Reports.Queries.List;
using ArchLens.Report.Domain.Entities.ReportEntities;
using ArchLens.Report.Domain.Interfaces.ReportInterfaces;
using ArchLens.Report.Domain.ValueObjects.Reports;
using ArchLens.SharedKernel.Application;
using FluentAssertions;
using NSubstitute;

namespace ArchLens.Report.Tests.Application.UseCases.Reports;

public class GetReportHandlerTests
{
    private readonly IReportRepository _repository = Substitute.For<IReportRepository>();

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
    public async Task GetById_Existing_ShouldReturnReport()
    {
        var report = CreateReport();
        _repository.GetByIdAsync(report.Id, Arg.Any<CancellationToken>()).Returns(report);

        var handler = new GetReportByIdHandler(_repository);
        var result = await handler.Handle(new GetReportByIdQuery(report.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Components.Should().HaveCount(1);
        result.Value.Risks.Should().HaveCount(1);
        result.Value.ProvidersUsed.Should().Contain("openai");
    }

    [Fact]
    public async Task GetById_NotFound_ShouldReturnFailure()
    {
        _repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((AnalysisReport?)null);

        var handler = new GetReportByIdHandler(_repository);
        var result = await handler.Handle(new GetReportByIdQuery(Guid.NewGuid()), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(Error.NotFound);
    }

    [Fact]
    public async Task GetByAnalysis_Existing_ShouldReturnReport()
    {
        var report = CreateReport();
        _repository.GetByAnalysisIdAsync(report.AnalysisId, Arg.Any<CancellationToken>()).Returns(report);

        var handler = new GetReportByAnalysisHandler(_repository);
        var result = await handler.Handle(new GetReportByAnalysisQuery(report.AnalysisId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.AnalysisId.Should().Be(report.AnalysisId);
    }

    [Fact]
    public async Task ListReports_ShouldReturnPagedResponse()
    {
        var reports = new List<AnalysisReport> { CreateReport(), CreateReport() };
        _repository.ListAsync(1, 20, Arg.Any<CancellationToken>()).Returns(reports);
        _repository.CountAsync(Arg.Any<CancellationToken>()).Returns(2L);

        var handler = new ListReportsHandler(_repository);
        var result = await handler.Handle(new ListReportsQuery(1, 20), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(2);
        result.Value.TotalCount.Should().Be(2);
    }
}

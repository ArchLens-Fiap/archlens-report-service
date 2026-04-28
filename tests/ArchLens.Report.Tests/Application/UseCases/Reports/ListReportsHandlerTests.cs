using ArchLens.Report.Application.UseCases.Reports.Queries.List;
using ArchLens.Report.Domain.Entities.ReportEntities;
using ArchLens.Report.Domain.Interfaces.ReportInterfaces;
using ArchLens.Report.Domain.ValueObjects.Reports;
using FluentAssertions;
using NSubstitute;

namespace ArchLens.Report.Tests.Application.UseCases.Reports;

public class ListReportsHandlerTests
{
    private readonly IReportRepository _repository = Substitute.For<IReportRepository>();
    private readonly ListReportsHandler _handler;

    public ListReportsHandlerTests()
    {
        _handler = new ListReportsHandler(_repository);
    }

    private static AnalysisReport CreateReport(int componentCount = 2, int riskCount = 1)
    {
        var components = Enumerable.Range(0, componentCount)
            .Select(i => new IdentifiedComponent($"Service{i}", "service", "desc", 0.9))
            .ToList();
        var risks = Enumerable.Range(0, riskCount)
            .Select(i => new ArchitectureRisk($"Risk{i}", "desc", "medium", "security", "fix"))
            .ToList();

        return AnalysisReport.Create(
            Guid.NewGuid(), Guid.NewGuid(),
            components, [],
            risks, [],
            new ArchitectureScores(7, 8, 6, 7),
            0.85, ["openai"], 1200);
    }

    [Fact]
    public async Task Handle_EmptyRepository_ShouldReturnEmptyPagedResponse()
    {
        _repository.ListAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns([]);
        _repository.CountAsync(Arg.Any<CancellationToken>()).Returns(0L);

        var result = await _handler.Handle(new ListReportsQuery(1, 10), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().BeEmpty();
        result.Value.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WithReports_ShouldReturnSummaries()
    {
        var reports = new[] { CreateReport(3, 2), CreateReport(1, 0) };
        _repository.ListAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((IReadOnlyList<AnalysisReport>)reports);
        _repository.CountAsync(Arg.Any<CancellationToken>()).Returns(2L);

        var result = await _handler.Handle(new ListReportsQuery(1, 10), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(2);
        result.Value.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_ShouldMapSummaryFields()
    {
        var report = CreateReport(componentCount: 4, riskCount: 3);
        _repository.ListAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((IReadOnlyList<AnalysisReport>)[report]);
        _repository.CountAsync(Arg.Any<CancellationToken>()).Returns(1L);

        var result = await _handler.Handle(new ListReportsQuery(1, 10), CancellationToken.None);

        var summary = result.Value.Items.Single();
        summary.Id.Should().Be(report.Id);
        summary.AnalysisId.Should().Be(report.AnalysisId);
        summary.DiagramId.Should().Be(report.DiagramId);
        summary.ComponentCount.Should().Be(4);
        summary.RiskCount.Should().Be(3);
        summary.ProvidersUsed.Should().Contain("openai");
    }

    [Fact]
    public async Task Handle_ShouldPassPaginationToRepository()
    {
        _repository.ListAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns([]);
        _repository.CountAsync(Arg.Any<CancellationToken>()).Returns(0L);

        await _handler.Handle(new ListReportsQuery(Page: 2, PageSize: 5), CancellationToken.None);

        await _repository.Received(1).ListAsync(2, 5, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnCorrectPageMetadata()
    {
        _repository.ListAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns([]);
        _repository.CountAsync(Arg.Any<CancellationToken>()).Returns(50L);

        var result = await _handler.Handle(new ListReportsQuery(Page: 3, PageSize: 10), CancellationToken.None);

        result.Value.Page.Should().Be(3);
        result.Value.PageSize.Should().Be(10);
        result.Value.TotalCount.Should().Be(50);
    }
}

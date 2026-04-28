using ArchLens.Report.Api.Controllers;
using ArchLens.Report.Application.Contracts.DTOs.ReportDTOs;
using ArchLens.Report.Application.UseCases.Reports.Queries.GetByAnalysis;
using ArchLens.Report.Application.UseCases.Reports.Queries.GetById;
using ArchLens.Report.Application.UseCases.Reports.Queries.List;
using ArchLens.Report.Domain.Interfaces.ReportInterfaces;
using ArchLens.SharedKernel.Application;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace ArchLens.Report.Tests.Api.Controllers;

public class ReportsControllerTests
{
    private readonly IMediator _mediator = Substitute.For<IMediator>();
    private readonly IReportRepository _reportRepo = Substitute.For<IReportRepository>();
    private readonly ReportsController _controller;

    public ReportsControllerTests()
    {
        _controller = new ReportsController(_mediator, _reportRepo);
    }

    private static ReportResponse CreateReportResponse()
    {
        return new ReportResponse(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            [new ComponentDto("GW", "gateway", "desc", 0.9)],
            [new ConnectionDto("A", "B", "HTTP", "call")],
            [new RiskDto("Risk", "desc", "high", "security", "fix")],
            ["rec"],
            new ScoresDto(7, 8, 6, 7),
            7.0, 0.85, ["openai"], 1200, DateTime.UtcNow);
    }

    [Fact]
    public async Task GetById_Found_ShouldReturnOk()
    {
        var response = CreateReportResponse();
        var id = response.Id;
        _mediator.Send(Arg.Any<GetReportByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(response));

        var result = await _controller.GetById(id, CancellationToken.None);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(response);
    }

    [Fact]
    public async Task GetById_NotFound_ShouldReturnNotFound()
    {
        _mediator.Send(Arg.Any<GetReportByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<ReportResponse>(Error.NotFound));

        var result = await _controller.GetById(Guid.NewGuid(), CancellationToken.None);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetByAnalysis_Found_ShouldReturnOk()
    {
        var response = CreateReportResponse();
        _mediator.Send(Arg.Any<GetReportByAnalysisQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(response));

        var result = await _controller.GetByAnalysis(Guid.NewGuid(), CancellationToken.None);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(response);
    }

    [Fact]
    public async Task GetByAnalysis_NotFound_ShouldReturnNotFound()
    {
        _mediator.Send(Arg.Any<GetReportByAnalysisQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<ReportResponse>(Error.NotFound));

        var result = await _controller.GetByAnalysis(Guid.NewGuid(), CancellationToken.None);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task List_ShouldReturnOk()
    {
        var summaries = new List<ReportSummaryResponse>
        {
            new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 7.0, 0.9, 3, 1, ["openai"], DateTime.UtcNow)
        };
        var paged = new PagedResponse<ReportSummaryResponse>(summaries, 1, 20, 1);
        _mediator.Send(Arg.Any<ListReportsQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(paged));

        var result = await _controller.List(1, 20, CancellationToken.None);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(paged);
    }

    [Fact]
    public async Task GetAdminMetrics_ShouldReturnOk()
    {
        var metrics = new AdminReportMetrics(
            10, 7.5,
            new Dictionary<string, int> { ["openai"] = 5 },
            new ScoreAverages(7, 8, 6, 7));
        _reportRepo.GetAdminMetricsAsync(Arg.Any<CancellationToken>()).Returns(metrics);

        var result = await _controller.GetAdminMetrics(CancellationToken.None);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(metrics);
    }

    [Fact]
    public async Task DeleteByAnalysis_Found_ShouldReturnNoContent()
    {
        _reportRepo.DeleteByAnalysisIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(true);

        var result = await _controller.DeleteByAnalysis(Guid.NewGuid(), CancellationToken.None);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteByAnalysis_NotFound_ShouldReturnNotFound()
    {
        _reportRepo.DeleteByAnalysisIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(false);

        var result = await _controller.DeleteByAnalysis(Guid.NewGuid(), CancellationToken.None);

        result.Should().BeOfType<NotFoundResult>();
    }
}

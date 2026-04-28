using ArchLens.Report.Application.UseCases.Reports.Queries.GetById;
using ArchLens.Report.Application.UseCases.Reports.Queries.GetByAnalysis;
using ArchLens.Report.Application.UseCases.Reports.Queries.List;
using ArchLens.Report.Domain.Interfaces.ReportInterfaces;
using ArchLens.SharedKernel.Application;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArchLens.Report.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public sealed class ReportsController(IMediator mediator, IReportRepository reportRepo) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetReportByIdQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpGet("analysis/{analysisId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByAnalysis(Guid analysisId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetReportByAnalysisQuery(analysisId), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await mediator.Send(new ListReportsQuery(page, pageSize), ct);
        return Ok(result.Value);
    }

    [HttpGet("admin/metrics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAdminMetrics(CancellationToken ct)
    {
        var metrics = await reportRepo.GetAdminMetricsAsync(ct);
        return Ok(metrics);
    }

    [HttpDelete("analysis/{analysisId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteByAnalysis(Guid analysisId, CancellationToken ct)
    {
        var deleted = await reportRepo.DeleteByAnalysisIdAsync(analysisId, ct);
        return deleted ? NoContent() : NotFound();
    }
}

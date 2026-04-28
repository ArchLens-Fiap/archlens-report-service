using ArchLens.Report.Application.Contracts.DTOs.ReportDTOs;
using ArchLens.Report.Domain.Interfaces.ReportInterfaces;
using ArchLens.SharedKernel.Application;
using MediatR;

namespace ArchLens.Report.Application.UseCases.Reports.Queries.List;

public sealed class ListReportsHandler(IReportRepository repository)
    : IRequestHandler<ListReportsQuery, Result<PagedResponse<ReportSummaryResponse>>>
{
    public async Task<Result<PagedResponse<ReportSummaryResponse>>> Handle(
        ListReportsQuery request,
        CancellationToken cancellationToken)
    {
        var paged = new PagedRequest(request.Page, request.PageSize);

        var reports = await repository.ListAsync(paged.Page, paged.PageSize, cancellationToken);
        var total = await repository.CountAsync(cancellationToken);

        var summaries = reports.Select(r => new ReportSummaryResponse(
            r.Id,
            r.AnalysisId,
            r.DiagramId,
            r.OverallScore,
            r.Confidence,
            r.Components.Count,
            r.Risks.Count,
            r.ProvidersUsed,
            r.CreatedAt)).ToList();

        return new PagedResponse<ReportSummaryResponse>(summaries, paged.Page, paged.PageSize, (int)total);
    }
}

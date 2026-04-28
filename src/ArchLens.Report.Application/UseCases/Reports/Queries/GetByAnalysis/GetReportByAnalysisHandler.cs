using ArchLens.Report.Application.Contracts.DTOs.ReportDTOs;
using ArchLens.Report.Application.UseCases.Reports;
using ArchLens.Report.Domain.Interfaces.ReportInterfaces;
using ArchLens.SharedKernel.Application;
using MediatR;

namespace ArchLens.Report.Application.UseCases.Reports.Queries.GetByAnalysis;

public sealed class GetReportByAnalysisHandler(IReportRepository repository)
    : IRequestHandler<GetReportByAnalysisQuery, Result<ReportResponse>>
{
    public async Task<Result<ReportResponse>> Handle(GetReportByAnalysisQuery request, CancellationToken cancellationToken)
    {
        var report = await repository.GetByAnalysisIdAsync(request.AnalysisId, cancellationToken);

        if (report is null)
            return Result.Failure<ReportResponse>(Error.NotFound);

        return ReportMapper.ToResponse(report);
    }
}

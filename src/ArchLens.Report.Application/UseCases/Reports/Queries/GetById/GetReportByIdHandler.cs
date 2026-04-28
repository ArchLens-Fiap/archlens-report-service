using ArchLens.Report.Application.Contracts.DTOs.ReportDTOs;
using ArchLens.Report.Application.UseCases.Reports;
using ArchLens.Report.Domain.Interfaces.ReportInterfaces;
using ArchLens.SharedKernel.Application;
using MediatR;

namespace ArchLens.Report.Application.UseCases.Reports.Queries.GetById;

public sealed class GetReportByIdHandler(IReportRepository repository)
    : IRequestHandler<GetReportByIdQuery, Result<ReportResponse>>
{
    public async Task<Result<ReportResponse>> Handle(GetReportByIdQuery request, CancellationToken cancellationToken)
    {
        var report = await repository.GetByIdAsync(request.Id, cancellationToken);

        if (report is null)
            return Result.Failure<ReportResponse>(Error.NotFound);

        return ReportMapper.ToResponse(report);
    }
}

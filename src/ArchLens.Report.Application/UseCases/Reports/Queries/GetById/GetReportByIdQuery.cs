using ArchLens.Report.Application.Contracts.DTOs.ReportDTOs;
using ArchLens.SharedKernel.Application;
using MediatR;

namespace ArchLens.Report.Application.UseCases.Reports.Queries.GetById;

public record GetReportByIdQuery(Guid Id) : IRequest<Result<ReportResponse>>;

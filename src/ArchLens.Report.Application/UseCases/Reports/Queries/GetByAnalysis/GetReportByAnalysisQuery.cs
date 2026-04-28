using ArchLens.Report.Application.Contracts.DTOs.ReportDTOs;
using ArchLens.SharedKernel.Application;
using MediatR;

namespace ArchLens.Report.Application.UseCases.Reports.Queries.GetByAnalysis;

public record GetReportByAnalysisQuery(Guid AnalysisId) : IRequest<Result<ReportResponse>>;

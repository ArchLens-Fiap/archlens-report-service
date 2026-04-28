using ArchLens.Report.Application.Contracts.DTOs.ReportDTOs;
using ArchLens.SharedKernel.Application;
using MediatR;

namespace ArchLens.Report.Application.UseCases.Reports.Queries.List;

public record ListReportsQuery(int Page = 1, int PageSize = 20, string? UserId = null, bool IsAdmin = false) : IRequest<Result<PagedResponse<ReportSummaryResponse>>>;

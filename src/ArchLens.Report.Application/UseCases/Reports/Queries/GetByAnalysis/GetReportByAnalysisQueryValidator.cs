using FluentValidation;

namespace ArchLens.Report.Application.UseCases.Reports.Queries.GetByAnalysis;

public sealed class GetReportByAnalysisQueryValidator : AbstractValidator<GetReportByAnalysisQuery>
{
    public GetReportByAnalysisQueryValidator()
    {
        RuleFor(x => x.AnalysisId)
            .NotEmpty()
            .WithMessage("AnalysisId is required.");
    }
}

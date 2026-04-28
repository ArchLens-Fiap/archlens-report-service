using FluentValidation;

namespace ArchLens.Report.Application.UseCases.Reports.Queries.GetById;

public sealed class GetReportByIdQueryValidator : AbstractValidator<GetReportByIdQuery>
{
    public GetReportByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required.");
    }
}

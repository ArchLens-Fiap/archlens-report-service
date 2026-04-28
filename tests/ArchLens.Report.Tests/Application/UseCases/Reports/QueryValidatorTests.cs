using ArchLens.Report.Application.UseCases.Reports.Queries.GetByAnalysis;
using ArchLens.Report.Application.UseCases.Reports.Queries.GetById;
using ArchLens.Report.Application.UseCases.Reports.Queries.List;
using FluentValidation.TestHelper;

namespace ArchLens.Report.Tests.Application.UseCases.Reports;

public class QueryValidatorTests
{
    // GetReportByIdQueryValidator

    private readonly GetReportByIdQueryValidator _byIdValidator = new();

    [Fact]
    public void GetById_ValidGuid_ShouldPass()
    {
        var result = _byIdValidator.TestValidate(new GetReportByIdQuery(Guid.NewGuid()));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void GetById_EmptyGuid_ShouldFail()
    {
        var result = _byIdValidator.TestValidate(new GetReportByIdQuery(Guid.Empty));

        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Id is required.");
    }

    // GetReportByAnalysisQueryValidator

    private readonly GetReportByAnalysisQueryValidator _byAnalysisValidator = new();

    [Fact]
    public void GetByAnalysis_ValidGuid_ShouldPass()
    {
        var result = _byAnalysisValidator.TestValidate(new GetReportByAnalysisQuery(Guid.NewGuid()));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void GetByAnalysis_EmptyGuid_ShouldFail()
    {
        var result = _byAnalysisValidator.TestValidate(new GetReportByAnalysisQuery(Guid.Empty));

        result.ShouldHaveValidationErrorFor(x => x.AnalysisId)
            .WithErrorMessage("AnalysisId is required.");
    }

    // ListReportsQueryValidator

    private readonly ListReportsQueryValidator _listValidator = new();

    [Fact]
    public void ListReports_ValidQuery_ShouldPass()
    {
        var result = _listValidator.TestValidate(new ListReportsQuery(1, 20));

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ListReports_PageZero_ShouldFail()
    {
        var result = _listValidator.TestValidate(new ListReportsQuery(0, 20));

        result.ShouldHaveValidationErrorFor(x => x.Page)
            .WithErrorMessage("Page must be greater than or equal to 1.");
    }

    [Fact]
    public void ListReports_PageSizeZero_ShouldFail()
    {
        var result = _listValidator.TestValidate(new ListReportsQuery(1, 0));

        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("PageSize must be between 1 and 100.");
    }

    [Fact]
    public void ListReports_PageSizeOverMax_ShouldFail()
    {
        var result = _listValidator.TestValidate(new ListReportsQuery(1, 101));

        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("PageSize must be between 1 and 100.");
    }

    [Fact]
    public void ListReports_PageSizeExactMax_ShouldPass()
    {
        var result = _listValidator.TestValidate(new ListReportsQuery(1, 100));

        result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void ListReports_NegativePage_ShouldFail()
    {
        var result = _listValidator.TestValidate(new ListReportsQuery(-1, 10));

        result.ShouldHaveValidationErrorFor(x => x.Page);
    }
}

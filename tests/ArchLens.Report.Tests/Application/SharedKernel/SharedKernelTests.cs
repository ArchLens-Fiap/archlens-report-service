using ArchLens.Report.Application.Contracts.DTOs.ReportDTOs;
using ArchLens.Report.Domain.Interfaces.ReportInterfaces;
using ArchLens.SharedKernel.Application;
using FluentAssertions;

namespace ArchLens.Report.Tests.Application.SharedKernel;

public class SharedKernelTests
{
    // Result tests

    [Fact]
    public void Result_Success_ShouldBeSuccess()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Result_Failure_ShouldBeFailure()
    {
        var result = Result.Failure(Error.NotFound);

        result.IsFailure.Should().BeTrue();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(Error.NotFound);
    }

    [Fact]
    public void ResultT_Success_ShouldHaveValue()
    {
        var result = Result.Success("hello");

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("hello");
    }

    [Fact]
    public void ResultT_Failure_AccessingValueShouldThrow()
    {
        var result = Result.Failure<string>(Error.NotFound);

        var act = () => result.Value;

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ResultT_ImplicitConversion_ShouldWork()
    {
        Result<string> result = "test";

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("test");
    }

    // Error tests

    [Fact]
    public void Error_Predefined_ShouldHaveCorrectCodes()
    {
        Error.None.Code.Should().BeEmpty();
        Error.NotFound.Code.Should().Be("Error.NotFound");
        Error.Conflict.Code.Should().Be("Error.Conflict");
        Error.Validation.Code.Should().Be("Error.Validation");
        Error.NullValue.Code.Should().Be("Error.NullValue");
    }

    // PagedRequest tests

    [Fact]
    public void PagedRequest_NegativePage_ShouldClampTo1()
    {
        var request = new PagedRequest(-5, 20);

        request.Page.Should().Be(1);
    }

    [Fact]
    public void PagedRequest_NegativePageSize_ShouldClampTo20()
    {
        var request = new PagedRequest(1, -5);

        request.PageSize.Should().Be(20);
    }

    [Fact]
    public void PagedRequest_OverMaxPageSize_ShouldClampTo100()
    {
        var request = new PagedRequest(1, 200);

        request.PageSize.Should().Be(100);
    }

    [Fact]
    public void PagedRequest_Skip_ShouldCalculateCorrectly()
    {
        var request = new PagedRequest(3, 10);

        request.Skip.Should().Be(20);
    }

    // PagedResponse tests

    [Fact]
    public void PagedResponse_TotalPages_ShouldCalculateCorrectly()
    {
        var items = new List<string> { "a", "b" };
        var response = new PagedResponse<string>(items, 1, 10, 25);

        response.TotalPages.Should().Be(3);
    }

    [Fact]
    public void PagedResponse_HasPrevious_FirstPage_ShouldBeFalse()
    {
        var response = new PagedResponse<string>([], 1, 10, 25);

        response.HasPrevious.Should().BeFalse();
    }

    [Fact]
    public void PagedResponse_HasPrevious_SecondPage_ShouldBeTrue()
    {
        var response = new PagedResponse<string>([], 2, 10, 25);

        response.HasPrevious.Should().BeTrue();
    }

    [Fact]
    public void PagedResponse_HasNext_LastPage_ShouldBeFalse()
    {
        var response = new PagedResponse<string>([], 3, 10, 25);

        response.HasNext.Should().BeFalse();
    }

    [Fact]
    public void PagedResponse_HasNext_FirstPage_ShouldBeTrue()
    {
        var response = new PagedResponse<string>([], 1, 10, 25);

        response.HasNext.Should().BeTrue();
    }

    // AdminReportMetrics / ScoreAverages tests

    [Fact]
    public void AdminReportMetrics_ShouldStoreAllValues()
    {
        var providers = new Dictionary<string, int> { ["openai"] = 5, ["gemini"] = 3 };
        var avgScores = new ScoreAverages(7.5, 8.0, 6.5, 7.0);
        var metrics = new AdminReportMetrics(8, 7.25, providers, avgScores);

        metrics.TotalReports.Should().Be(8);
        metrics.AverageOverallScore.Should().Be(7.25);
        metrics.ProviderUsage.Should().ContainKey("openai");
        metrics.ProviderUsage["openai"].Should().Be(5);
        metrics.AverageScores.Scalability.Should().Be(7.5);
        metrics.AverageScores.Security.Should().Be(8.0);
        metrics.AverageScores.Reliability.Should().Be(6.5);
        metrics.AverageScores.Maintainability.Should().Be(7.0);
    }

    [Fact]
    public void ScoreAverages_Equality_ShouldWork()
    {
        var a = new ScoreAverages(7, 8, 6, 7);
        var b = new ScoreAverages(7, 8, 6, 7);

        a.Should().Be(b);
    }

    // ReportSummaryResponse tests

    [Fact]
    public void ReportSummaryResponse_ShouldStoreAllProperties()
    {
        var id = Guid.NewGuid();
        var analysisId = Guid.NewGuid();
        var diagramId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;
        var response = new ReportSummaryResponse(id, analysisId, diagramId, 7.5, 0.9, 5, 2, ["openai"], createdAt);

        response.Id.Should().Be(id);
        response.AnalysisId.Should().Be(analysisId);
        response.DiagramId.Should().Be(diagramId);
        response.OverallScore.Should().Be(7.5);
        response.Confidence.Should().Be(0.9);
        response.ComponentCount.Should().Be(5);
        response.RiskCount.Should().Be(2);
        response.ProvidersUsed.Should().Contain("openai");
        response.CreatedAt.Should().Be(createdAt);
    }
}

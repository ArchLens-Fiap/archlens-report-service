using System.Net;
using System.Text.Json;
using ArchLens.Report.Api.ExceptionHandlers;
using ArchLens.SharedKernel.Domain;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace ArchLens.Report.Tests.Api.ExceptionHandlers;

public class GlobalExceptionHandlerTests
{
    private readonly GlobalExceptionHandler _handler;
    private readonly ILogger<GlobalExceptionHandler> _logger = Substitute.For<ILogger<GlobalExceptionHandler>>();

    public GlobalExceptionHandlerTests()
    {
        _handler = new GlobalExceptionHandler(_logger);
    }

    private static HttpContext CreateHttpContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }

    [Fact]
    public async Task TryHandleAsync_ValidationException_ShouldReturn400()
    {
        var context = CreateHttpContext();
        var failures = new List<ValidationFailure>
        {
            new("Name", "Name is required"),
            new("Age", "Age must be positive")
        };
        var exception = new ValidationException(failures);

        var result = await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        result.Should().BeTrue();
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task TryHandleAsync_DomainException_ShouldReturn422()
    {
        var context = CreateHttpContext();
        var exception = new TestDomainException("TEST_CODE", "Something went wrong in domain");

        var result = await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        result.Should().BeTrue();
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task TryHandleAsync_UnhandledException_ShouldReturn500()
    {
        var context = CreateHttpContext();
        var exception = new InvalidOperationException("Unexpected error");

        var result = await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        result.Should().BeTrue();
        context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task TryHandleAsync_UnhandledException_ShouldLogError()
    {
        var context = CreateHttpContext();
        var exception = new InvalidOperationException("Unexpected error");

        await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        _logger.ReceivedCalls().Should().NotBeEmpty();
    }

    [Fact]
    public async Task TryHandleAsync_ValidationException_ShouldWriteJsonBody()
    {
        var context = CreateHttpContext();
        var failures = new List<ValidationFailure> { new("Field", "Error message") };
        var exception = new ValidationException(failures);

        await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        body.Should().Contain("Validation Error");
    }

    private sealed class TestDomainException : DomainException
    {
        public TestDomainException(string code, string message) : base(code, message) { }
    }
}

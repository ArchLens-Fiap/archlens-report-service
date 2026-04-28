using ArchLens.Report.Application.Behaviors;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using NSubstitute;

namespace ArchLens.Report.Tests.Application.Behaviors;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_NoValidators_ShouldCallNext()
    {
        var validators = Array.Empty<IValidator<TestRequest>>();
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);
        var expectedResponse = new TestResponse("ok");

        var result = await behavior.Handle(
            new TestRequest("test"),
            () => Task.FromResult(expectedResponse),
            CancellationToken.None);

        result.Should().Be(expectedResponse);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldCallNext()
    {
        var validator = Substitute.For<IValidator<TestRequest>>();
        validator.ValidateAsync(Arg.Any<ValidationContext<TestRequest>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        var behavior = new ValidationBehavior<TestRequest, TestResponse>([validator]);
        var expectedResponse = new TestResponse("ok");

        var result = await behavior.Handle(
            new TestRequest("test"),
            () => Task.FromResult(expectedResponse),
            CancellationToken.None);

        result.Should().Be(expectedResponse);
    }

    [Fact]
    public async Task Handle_InvalidRequest_ShouldThrowValidationException()
    {
        var validator = Substitute.For<IValidator<TestRequest>>();
        var failures = new List<ValidationFailure> { new("Value", "Value is required") };
        validator.ValidateAsync(Arg.Any<ValidationContext<TestRequest>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(failures));

        var behavior = new ValidationBehavior<TestRequest, TestResponse>([validator]);

        var act = () => behavior.Handle(
            new TestRequest(""),
            () => Task.FromResult(new TestResponse("should not reach")),
            CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Handle_MultipleValidators_AllValid_ShouldCallNext()
    {
        var validator1 = Substitute.For<IValidator<TestRequest>>();
        var validator2 = Substitute.For<IValidator<TestRequest>>();
        validator1.ValidateAsync(Arg.Any<ValidationContext<TestRequest>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());
        validator2.ValidateAsync(Arg.Any<ValidationContext<TestRequest>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        var behavior = new ValidationBehavior<TestRequest, TestResponse>([validator1, validator2]);
        var expectedResponse = new TestResponse("ok");

        var result = await behavior.Handle(
            new TestRequest("test"),
            () => Task.FromResult(expectedResponse),
            CancellationToken.None);

        result.Should().Be(expectedResponse);
    }

    [Fact]
    public async Task Handle_MultipleValidators_OneInvalid_ShouldThrow()
    {
        var validator1 = Substitute.For<IValidator<TestRequest>>();
        var validator2 = Substitute.For<IValidator<TestRequest>>();
        validator1.ValidateAsync(Arg.Any<ValidationContext<TestRequest>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());
        validator2.ValidateAsync(Arg.Any<ValidationContext<TestRequest>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[] { new ValidationFailure("Field", "Error") }));

        var behavior = new ValidationBehavior<TestRequest, TestResponse>([validator1, validator2]);

        var act = () => behavior.Handle(
            new TestRequest("test"),
            () => Task.FromResult(new TestResponse("nope")),
            CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }

    public record TestRequest(string Value) : IRequest<TestResponse>;
    public record TestResponse(string Result);
}

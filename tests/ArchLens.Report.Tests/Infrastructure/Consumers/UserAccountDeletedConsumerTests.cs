using ArchLens.Contracts.Events;
using ArchLens.Report.Domain.Interfaces.ReportInterfaces;
using ArchLens.Report.Infrastructure.Consumers;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace ArchLens.Report.Tests.Infrastructure.Consumers;

public class UserAccountDeletedConsumerTests
{
    private readonly IReportRepository _repository = Substitute.For<IReportRepository>();
    private readonly ILogger<UserAccountDeletedConsumer> _logger = Substitute.For<ILogger<UserAccountDeletedConsumer>>();
    private readonly UserAccountDeletedConsumer _consumer;

    public UserAccountDeletedConsumerTests()
    {
        _consumer = new UserAccountDeletedConsumer(_repository, _logger);
    }

    [Fact]
    public async Task Consume_ShouldCallDeleteByUserId()
    {
        var userId = Guid.NewGuid();
        var message = new UserAccountDeletedEvent
        {
            UserId = userId,
            Timestamp = DateTime.UtcNow
        };
        var context = Substitute.For<ConsumeContext<UserAccountDeletedEvent>>();
        context.Message.Returns(message);
        context.CancellationToken.Returns(CancellationToken.None);

        await _consumer.Consume(context);

        await _repository.Received(1).DeleteByUserIdAsync(userId.ToString(), Arg.Any<CancellationToken>());
    }
}

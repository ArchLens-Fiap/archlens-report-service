using ArchLens.Contracts.Events;
using ArchLens.Report.Domain.Interfaces.ReportInterfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ArchLens.Report.Infrastructure.Consumers;

public sealed class UserAccountDeletedConsumer(
    IReportRepository reportRepository,
    ILogger<UserAccountDeletedConsumer> logger)
    : IConsumer<UserAccountDeletedEvent>
{
    public async Task Consume(ConsumeContext<UserAccountDeletedEvent> context)
    {
        var userId = context.Message.UserId.ToString();
        logger.LogInformation("Deleting reports for user {UserId}", context.Message.UserId);

        await reportRepository.DeleteByUserIdAsync(userId, context.CancellationToken);

        logger.LogInformation("Reports deleted for user {UserId}", context.Message.UserId);
    }
}

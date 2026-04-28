using ArchLens.Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ArchLens.Report.Infrastructure.Consumers;

public sealed class GenerateReportFaultConsumer(
    ILogger<GenerateReportFaultConsumer> logger) : IConsumer<Fault<GenerateReportCommand>>
{
    public async Task Consume(ConsumeContext<Fault<GenerateReportCommand>> context)
    {
        var original = context.Message.Message;
        var errorMessage = context.Message.Exceptions.FirstOrDefault()?.Message ?? "Unknown error";

        logger.LogError(
            "Report generation failed for analysis {AnalysisId} after all retries: {Error}",
            original.AnalysisId, errorMessage);

        await context.Publish(new ReportFailedEvent
        {
            AnalysisId = original.AnalysisId,
            DiagramId = original.DiagramId,
            ErrorMessage = errorMessage,
            Timestamp = DateTime.UtcNow
        });
    }
}

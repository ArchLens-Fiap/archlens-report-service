using ArchLens.Report.Domain.Entities.ReportEntities;

namespace ArchLens.Report.Domain.Interfaces.ReportInterfaces;

public interface IReportRepository
{
    Task<AnalysisReport?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<AnalysisReport?> GetByAnalysisIdAsync(Guid analysisId, CancellationToken ct = default);
    Task<IReadOnlyList<AnalysisReport>> ListAsync(int page, int pageSize, CancellationToken ct = default);
    Task<long> CountAsync(CancellationToken ct = default);
    Task AddAsync(AnalysisReport report, CancellationToken ct = default);
    Task DeleteByUserIdAsync(string userId, CancellationToken ct = default);
    Task<bool> DeleteByAnalysisIdAsync(Guid analysisId, CancellationToken ct = default);
    Task<AdminReportMetrics> GetAdminMetricsAsync(CancellationToken ct = default);
}

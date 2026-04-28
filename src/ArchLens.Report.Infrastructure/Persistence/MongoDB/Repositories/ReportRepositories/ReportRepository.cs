using ArchLens.Report.Domain.Entities.ReportEntities;
using ArchLens.Report.Domain.Interfaces.ReportInterfaces;
using ArchLens.Report.Infrastructure.Persistence.MongoDB.Documents;
using MongoDB.Driver;

namespace ArchLens.Report.Infrastructure.Persistence.MongoDB.Repositories.ReportRepositories;

public sealed class ReportRepository : IReportRepository
{
    private readonly IMongoCollection<ReportDocument> _collection;

    public ReportRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<ReportDocument>("reports");
    }

    public async Task<AnalysisReport?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var doc = await _collection.Find(x => x.Id == id).FirstOrDefaultAsync(ct);
        return doc is null ? null : ReportDocumentMapper.ToDomain(doc);
    }

    public async Task<AnalysisReport?> GetByAnalysisIdAsync(Guid analysisId, CancellationToken ct = default)
    {
        var doc = await _collection.Find(x => x.AnalysisId == analysisId).FirstOrDefaultAsync(ct);
        return doc is null ? null : ReportDocumentMapper.ToDomain(doc);
    }

    public async Task<IReadOnlyList<AnalysisReport>> ListAsync(int page, int pageSize, CancellationToken ct = default, string? userId = null)
    {
        var filter = userId is not null
            ? Builders<ReportDocument>.Filter.Eq(x => x.UserId, userId)
            : FilterDefinition<ReportDocument>.Empty;

        var docs = await _collection
            .Find(filter)
            .SortByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(ct);

        return docs.Select(ReportDocumentMapper.ToDomain).ToList();
    }

    public async Task<long> CountAsync(CancellationToken ct = default, string? userId = null)
    {
        var filter = userId is not null
            ? Builders<ReportDocument>.Filter.Eq(x => x.UserId, userId)
            : FilterDefinition<ReportDocument>.Empty;

        return await _collection.CountDocumentsAsync(filter, cancellationToken: ct);
    }

    public async Task AddAsync(AnalysisReport report, CancellationToken ct = default)
    {
        var doc = ReportDocumentMapper.ToDocument(report);
        await _collection.InsertOneAsync(doc, cancellationToken: ct);
    }

    public async Task DeleteByUserIdAsync(string userId, CancellationToken ct = default)
    {
        await _collection.DeleteManyAsync(x => x.UserId == userId, cancellationToken: ct);
    }

    public async Task<bool> DeleteByAnalysisIdAsync(Guid analysisId, CancellationToken ct = default)
    {
        var result = await _collection.DeleteOneAsync(x => x.AnalysisId == analysisId, cancellationToken: ct);
        return result.DeletedCount > 0;
    }

    public async Task<AdminReportMetrics> GetAdminMetricsAsync(CancellationToken ct = default)
    {
        var docs = await _collection
            .Find(FilterDefinition<ReportDocument>.Empty)
            .ToListAsync(ct);

        var totalReports = docs.Count;

        if (totalReports == 0)
        {
            return new AdminReportMetrics(
                0,
                0,
                new Dictionary<string, int>(),
                new ScoreAverages(0, 0, 0, 0));
        }

        var averageOverallScore = docs.Average(d => d.OverallScore);

        var providerUsage = docs
            .SelectMany(d => d.ProvidersUsed)
            .GroupBy(p => p)
            .ToDictionary(g => g.Key, g => g.Count());

        var averageScores = new ScoreAverages(
            Scalability: docs.Average(d => d.Scores.Scalability),
            Security: docs.Average(d => d.Scores.Security),
            Reliability: docs.Average(d => d.Scores.Reliability),
            Maintainability: docs.Average(d => d.Scores.Maintainability));

        return new AdminReportMetrics(
            totalReports,
            averageOverallScore,
            providerUsage,
            averageScores);
    }
}

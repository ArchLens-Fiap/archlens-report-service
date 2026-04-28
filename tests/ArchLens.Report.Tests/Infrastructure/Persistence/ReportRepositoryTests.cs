using ArchLens.Report.Domain.Entities.ReportEntities;
using ArchLens.Report.Domain.ValueObjects.Reports;
using ArchLens.Report.Infrastructure.Persistence.MongoDB.Documents;
using ArchLens.Report.Infrastructure.Persistence.MongoDB.Repositories.ReportRepositories;
using FluentAssertions;
using MongoDB.Driver;
using NSubstitute;

namespace ArchLens.Report.Tests.Infrastructure.Persistence;

public class ReportRepositoryTests
{
    private readonly IMongoCollection<ReportDocument> _collection;
    private readonly IMongoDatabase _database;
    private readonly ReportRepository _repository;

    public ReportRepositoryTests()
    {
        _collection = Substitute.For<IMongoCollection<ReportDocument>>();
        _database = Substitute.For<IMongoDatabase>();
        _database.GetCollection<ReportDocument>("reports").Returns(_collection);
        _repository = new ReportRepository(_database);
    }

    private static AnalysisReport CreateTestReport(string? userId = "user-123")
    {
        return AnalysisReport.Create(
            Guid.NewGuid(), Guid.NewGuid(),
            [new IdentifiedComponent("Svc", "api", "desc", 0.9)],
            [new IdentifiedConnection("A", "B", "HTTP", "call")],
            [new ArchitectureRisk("Risk", "desc", "high", "security", "fix")],
            ["Cache data"],
            new ArchitectureScores(7, 8, 6, 7),
            0.85, ["openai"], 1200, userId);
    }

    [Fact]
    public void Constructor_ShouldGetReportsCollection()
    {
        _database.Received(1).GetCollection<ReportDocument>("reports");
    }

    [Fact]
    public async Task AddAsync_ShouldCallInsertOneAsync()
    {
        var report = CreateTestReport();

        await _repository.AddAsync(report);

        await _collection.Received(1).InsertOneAsync(
            Arg.Is<ReportDocument>(d => d.Id == report.Id),
            Arg.Any<InsertOneOptions>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddAsync_WithCancellationToken_ShouldForwardToken()
    {
        var report = CreateTestReport();
        using var cts = new CancellationTokenSource();

        await _repository.AddAsync(report, cts.Token);

        await _collection.Received(1).InsertOneAsync(
            Arg.Any<ReportDocument>(),
            Arg.Any<InsertOneOptions>(),
            cts.Token);
    }

    [Fact]
    public async Task CountAsync_ShouldCallCountDocumentsAsync()
    {
        _collection.CountDocumentsAsync(
            Arg.Any<FilterDefinition<ReportDocument>>(),
            Arg.Any<CountOptions>(),
            Arg.Any<CancellationToken>()).Returns(42L);

        var count = await _repository.CountAsync();

        count.Should().Be(42L);
    }

    [Fact]
    public async Task CountAsync_WithCancellationToken_ShouldForwardToken()
    {
        using var cts = new CancellationTokenSource();
        _collection.CountDocumentsAsync(
            Arg.Any<FilterDefinition<ReportDocument>>(),
            Arg.Any<CountOptions>(),
            Arg.Any<CancellationToken>()).Returns(0L);

        await _repository.CountAsync(cts.Token);

        await _collection.Received(1).CountDocumentsAsync(
            Arg.Any<FilterDefinition<ReportDocument>>(),
            Arg.Any<CountOptions>(),
            cts.Token);
    }

    [Fact]
    public async Task DeleteByUserIdAsync_ShouldCallDeleteManyAsync()
    {
        await _repository.DeleteByUserIdAsync("user-abc");

        await _collection.Received(1).DeleteManyAsync(
            Arg.Any<FilterDefinition<ReportDocument>>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteByAnalysisIdAsync_WhenDeleted_ShouldReturnTrue()
    {
        var deleteResult = Substitute.For<DeleteResult>();
        deleteResult.DeletedCount.Returns(1L);
        _collection.DeleteOneAsync(
            Arg.Any<FilterDefinition<ReportDocument>>(),
            Arg.Any<CancellationToken>()).Returns(deleteResult);

        var result = await _repository.DeleteByAnalysisIdAsync(Guid.NewGuid());

        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteByAnalysisIdAsync_WhenNotFound_ShouldReturnFalse()
    {
        var deleteResult = Substitute.For<DeleteResult>();
        deleteResult.DeletedCount.Returns(0L);
        _collection.DeleteOneAsync(
            Arg.Any<FilterDefinition<ReportDocument>>(),
            Arg.Any<CancellationToken>()).Returns(deleteResult);

        var result = await _repository.DeleteByAnalysisIdAsync(Guid.NewGuid());

        result.Should().BeFalse();
    }
}

using System.Text.Json;
using ArchLens.Report.Domain.Entities.ReportEntities;
using ArchLens.Report.Domain.ValueObjects.Reports;
using ArchLens.Report.Tests.BDD.Hooks;
using FluentAssertions;
using NSubstitute;
using Reqnroll;

namespace ArchLens.Report.Tests.BDD.StepDefinitions;

[Binding]
public class RelatorioSteps
{
    private readonly ScenarioContext _scenarioContext;
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private static readonly string[] DefaultRecommendations = ["Use caching"];
    private static readonly string[] DefaultProviders = ["OpenAI"];

    public RelatorioSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
        _client = scenarioContext.Get<HttpClient>("HttpClient");
    }

    private static AnalysisReport CreateFakeReport(Guid id, Guid analysisId)
    {
        return AnalysisReport.Reconstitute(
            id: id,
            analysisId: analysisId,
            diagramId: Guid.NewGuid(),
            components: Array.Empty<IdentifiedComponent>(),
            connections: Array.Empty<IdentifiedConnection>(),
            risks: Array.Empty<ArchitectureRisk>(),
            recommendations: DefaultRecommendations,
            scores: new ArchitectureScores(8.0, 7.5, 9.0, 8.5),
            overallScore: 8.25,
            confidence: 0.95,
            providersUsed: DefaultProviders,
            processingTimeMs: 1500,
            createdAt: DateTime.UtcNow,
            userId: "test-user");
    }

    [Given(@"que existe um relatório com ID ""(.*)""")]
    public void DadoQueExisteUmRelatorioComId(string id)
    {
        var guid = Guid.Parse(id);
        var report = CreateFakeReport(guid, Guid.NewGuid());

        TestHooks.MockReportRepo
            .GetByIdAsync(guid, Arg.Any<CancellationToken>())
            .Returns(report);
    }

    [Given(@"que não existe relatório com ID ""(.*)""")]
    public void DadoQueNaoExisteRelatorioComId(string id)
    {
        var guid = Guid.Parse(id);
        TestHooks.MockReportRepo
            .GetByIdAsync(guid, Arg.Any<CancellationToken>())
            .Returns((AnalysisReport?)null);
    }

    [Given(@"que existe um relatório para a análise ""(.*)""")]
    public void DadoQueExisteUmRelatorioParaAAnalise(string analysisId)
    {
        var guid = Guid.Parse(analysisId);
        var report = CreateFakeReport(Guid.NewGuid(), guid);

        TestHooks.MockReportRepo
            .GetByAnalysisIdAsync(guid, Arg.Any<CancellationToken>())
            .Returns(report);
    }

    [Given(@"que não existe relatório para a análise ""(.*)""")]
    public void DadoQueNaoExisteRelatorioParaAAnalise(string analysisId)
    {
        var guid = Guid.Parse(analysisId);
        TestHooks.MockReportRepo
            .GetByAnalysisIdAsync(guid, Arg.Any<CancellationToken>())
            .Returns((AnalysisReport?)null);
    }

    [Given(@"que existem relatórios cadastrados")]
    public void DadoQueExistemRelatoriosCadastrados()
    {
        var reports = new List<AnalysisReport>
        {
            CreateFakeReport(Guid.NewGuid(), Guid.NewGuid()),
            CreateFakeReport(Guid.NewGuid(), Guid.NewGuid())
        };

        TestHooks.MockReportRepo
            .ListAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(reports.AsReadOnly());

        TestHooks.MockReportRepo
            .CountAsync(Arg.Any<CancellationToken>())
            .Returns(2L);
    }

    [When(@"eu consultar o relatório pelo ID ""(.*)""")]
    public async Task QuandoEuConsultarORelatorioPeloId(string id)
    {
        var response = await _client.GetAsync($"/reports/{id}");
        _scenarioContext["Response"] = response;
        _scenarioContext["ResponseBody"] = await response.Content.ReadAsStringAsync();
    }

    [When(@"eu consultar o relatório pela análise ""(.*)""")]
    public async Task QuandoEuConsultarORelatorioPelaAnalise(string analysisId)
    {
        var response = await _client.GetAsync($"/reports/analysis/{analysisId}");
        _scenarioContext["Response"] = response;
        _scenarioContext["ResponseBody"] = await response.Content.ReadAsStringAsync();
    }

    [When(@"eu listar os relatórios")]
    public async Task QuandoEuListarOsRelatorios()
    {
        var response = await _client.GetAsync("/reports");
        _scenarioContext["Response"] = response;
        _scenarioContext["ResponseBody"] = await response.Content.ReadAsStringAsync();
    }

    [When(@"eu listar os relatórios com página (.*) e tamanho (.*)")]
    public async Task QuandoEuListarOsRelatoriosComPaginaETamanho(int page, int pageSize)
    {
        var response = await _client.GetAsync($"/reports?page={page}&pageSize={pageSize}");
        _scenarioContext["Response"] = response;
        _scenarioContext["ResponseBody"] = await response.Content.ReadAsStringAsync();
    }

    [Then(@"a resposta deve conter o relatório com ID ""(.*)""")]
    public void EntaoARespostaDeveConterORelatorioComId(string id)
    {
        var body = _scenarioContext.Get<string>("ResponseBody");
        body.Should().Contain(id.ToLowerInvariant());
    }

    [Then(@"a resposta deve conter a análise ""(.*)""")]
    public void EntaoARespostaDeveConterAAnalise(string analysisId)
    {
        var body = _scenarioContext.Get<string>("ResponseBody");
        body.Should().Contain(analysisId.ToLowerInvariant());
    }
}

using System.Text.Json;
using ArchLens.Report.Domain.Interfaces.ReportInterfaces;
using ArchLens.Report.Tests.BDD.Hooks;
using FluentAssertions;
using NSubstitute;
using Reqnroll;

namespace ArchLens.Report.Tests.BDD.StepDefinitions;

[Binding]
public class MetricasSteps
{
    private readonly ScenarioContext _scenarioContext;
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public MetricasSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
        _client = scenarioContext.Get<HttpClient>("HttpClient");
    }

    [Given(@"que existem métricas de relatórios disponíveis")]
    public void DadoQueExistemMetricasDeRelatoriosDisponiveis()
    {
        var metrics = new AdminReportMetrics(
            TotalReports: 50,
            AverageOverallScore: 8.2,
            ProviderUsage: new Dictionary<string, int>
            {
                { "OpenAI", 30 },
                { "Gemini", 20 }
            },
            AverageScores: new ScoreAverages(
                Scalability: 8.0,
                Security: 7.5,
                Reliability: 8.5,
                Maintainability: 8.8));

        TestHooks.MockReportRepo
            .GetAdminMetricsAsync(Arg.Any<CancellationToken>())
            .Returns(metrics);
    }

    [Given(@"que existem métricas com score médio de (.*)")]
    public void DadoQueExistemMetricasComScoreMedioDe(double score)
    {
        var metrics = new AdminReportMetrics(
            TotalReports: 10,
            AverageOverallScore: score,
            ProviderUsage: new Dictionary<string, int>
            {
                { "OpenAI", 10 }
            },
            AverageScores: new ScoreAverages(
                Scalability: score,
                Security: score,
                Reliability: score,
                Maintainability: score));

        TestHooks.MockReportRepo
            .GetAdminMetricsAsync(Arg.Any<CancellationToken>())
            .Returns(metrics);
    }

    [When(@"eu consultar as métricas administrativas de relatórios")]
    public async Task QuandoEuConsultarAsMetricasAdministrativasDeRelatorios()
    {
        var response = await _client.GetAsync("/reports/admin/metrics");
        _scenarioContext["Response"] = response;
        _scenarioContext["ResponseBody"] = await response.Content.ReadAsStringAsync();
    }

    [Then(@"a resposta deve conter as métricas de relatórios")]
    public void EntaoARespostaDeveConterAsMetricasDeRelatorios()
    {
        var body = _scenarioContext.Get<string>("ResponseBody");
        var metrics = JsonSerializer.Deserialize<AdminReportMetrics>(body, JsonOptions);
        metrics.Should().NotBeNull();
        metrics!.TotalReports.Should().BeGreaterThan(0);
        metrics.ProviderUsage.Should().NotBeEmpty();
    }

    [Then(@"a resposta deve conter score médio de (.*)")]
    public void EntaoARespostaDeveConterScoreMedioDe(double score)
    {
        var body = _scenarioContext.Get<string>("ResponseBody");
        var metrics = JsonSerializer.Deserialize<AdminReportMetrics>(body, JsonOptions);
        metrics.Should().NotBeNull();
        metrics!.AverageOverallScore.Should().Be(score);
    }

    [Then(@"a resposta deve conter uso de providers")]
    public void EntaoARespostaDeveConterUsoDeProviders()
    {
        var body = _scenarioContext.Get<string>("ResponseBody");
        var metrics = JsonSerializer.Deserialize<AdminReportMetrics>(body, JsonOptions);
        metrics.Should().NotBeNull();
        metrics!.ProviderUsage.Should().NotBeEmpty();
    }
}

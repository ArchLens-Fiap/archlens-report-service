using System.Security.Claims;
using System.Text.Encodings.Web;
using ArchLens.Report.Domain.Interfaces.ReportInterfaces;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NSubstitute;
using Reqnroll;

namespace ArchLens.Report.Tests.BDD.Hooks;

[Binding]
public sealed class TestHooks
{
    private static BddWebApplicationFactory _factory = null!;
    private static HttpClient _client = null!;

    internal static IReportRepository MockReportRepo { get; private set; } = null!;

    [BeforeTestRun]
    public static void BeforeTestRun()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        Environment.SetEnvironmentVariable("MongoDB__ConnectionString", "mongodb://localhost:27017");
        Environment.SetEnvironmentVariable("MongoDB__DatabaseName", "archlens_report_bdd_test");
        Environment.SetEnvironmentVariable("RabbitMQ__Host", "localhost");
        Environment.SetEnvironmentVariable("RabbitMQ__Username", "guest");
        Environment.SetEnvironmentVariable("RabbitMQ__Password", "guest");
        Environment.SetEnvironmentVariable("Jwt__Key", "bdd-test-jwt-secret-key-minimum-32-characters!");

        MockReportRepo = Substitute.For<IReportRepository>();
        _factory = new BddWebApplicationFactory(MockReportRepo);
        _client = _factory.CreateClient();
    }

    [AfterTestRun]
    public static void AfterTestRun()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [BeforeScenario]
    public void BeforeScenario(ScenarioContext scenarioContext)
    {
        BddTestAuthHandler.Reset();
        MockReportRepo.ClearReceivedCalls();
        scenarioContext.Set(_client, "HttpClient");
        scenarioContext.Set(_factory, "Factory");
    }
}

public sealed class BddWebApplicationFactory : WebApplicationFactory<ArchLens.Report.Api.Program>
{
    private readonly IReportRepository _mockReportRepo;

    public BddWebApplicationFactory(IReportRepository mockReportRepo)
    {
        _mockReportRepo = mockReportRepo;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Replace MongoDB with mocks
            var mongoDescriptors = services
                .Where(d =>
                    d.ServiceType.FullName?.Contains("Mongo") == true
                    || d.ServiceType == typeof(IMongoClient)
                    || d.ServiceType == typeof(IMongoDatabase))
                .ToList();
            foreach (var descriptor in mongoDescriptors)
                services.Remove(descriptor);

            var mockMongoClient = Substitute.For<IMongoClient>();
            var mockMongoDatabase = Substitute.For<IMongoDatabase>();
            mockMongoClient.GetDatabase(Arg.Any<string>(), Arg.Any<MongoDatabaseSettings>())
                .Returns(mockMongoDatabase);
            services.AddSingleton(mockMongoClient);
            services.AddScoped<IMongoDatabase>(_ => mockMongoDatabase);

            // Replace MassTransit with TestHarness
            var massTransitDescriptors = services
                .Where(d => d.ServiceType.FullName?.Contains("MassTransit") == true)
                .ToList();
            foreach (var descriptor in massTransitDescriptors)
                services.Remove(descriptor);

            services.AddMassTransitTestHarness();

            // Remove HostedServices
            var hostedServices = services
                .Where(d => d.ServiceType == typeof(Microsoft.Extensions.Hosting.IHostedService))
                .ToList();
            foreach (var descriptor in hostedServices)
                services.Remove(descriptor);

            // Replace IReportRepository with mock
            var repoDescriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(IReportRepository));
            if (repoDescriptor is not null)
                services.Remove(repoDescriptor);
            services.AddSingleton(_mockReportRepo);

            // Replace auth with test handler
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = "Test";
                options.DefaultScheme = "Test";
            }).AddScheme<AuthenticationSchemeOptions, BddTestAuthHandler>("Test", _ => { });

            services.AddAuthorization();
        });
    }
}

public sealed class BddTestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private static bool _isAuthenticated;
    private static string _role = "User";

    public BddTestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder) { }

    public static void SetAuthenticated(string role = "User")
    {
        _isAuthenticated = true;
        _role = role;
    }

    public static void Reset()
    {
        _isAuthenticated = false;
        _role = "User";
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!_isAuthenticated)
            return Task.FromResult(AuthenticateResult.NoResult());

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "bdd-test-user"),
            new Claim(ClaimTypes.Name, "BDD Test User"),
            new Claim(ClaimTypes.Email, "bdd@test.com"),
            new Claim(ClaimTypes.Role, _role),
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

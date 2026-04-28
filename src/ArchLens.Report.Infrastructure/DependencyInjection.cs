using ArchLens.Report.Domain.Interfaces.ReportInterfaces;
using ArchLens.Report.Infrastructure.Consumers;
using ArchLens.Report.Infrastructure.Persistence.MongoDB.Repositories.ReportRepositories;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace ArchLens.Report.Infrastructure;

public static class DependencyInjection
{
    private static bool _mongoRegistered;

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMongoDB(configuration);
        services.AddMessaging(configuration);
        return services;
    }

    private static void AddMongoDB(this IServiceCollection services, IConfiguration configuration)
    {
        if (!_mongoRegistered)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            _mongoRegistered = true;
        }

        var mongoSection = configuration.GetRequiredSection("MongoDB");
        var connectionString = mongoSection["ConnectionString"]
            ?? throw new InvalidOperationException("Configuration 'MongoDB:ConnectionString' is required");
        var databaseName = mongoSection["DatabaseName"]
            ?? throw new InvalidOperationException("Configuration 'MongoDB:DatabaseName' is required");

        services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
        services.AddScoped(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(databaseName));
        services.AddScoped<IReportRepository, ReportRepository>();
    }

    private static void AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitSection = configuration.GetRequiredSection("RabbitMQ");
        var host = rabbitSection["Host"] ?? throw new InvalidOperationException("Configuration 'RabbitMQ:Host' is required");
        var username = rabbitSection["Username"] ?? throw new InvalidOperationException("Configuration 'RabbitMQ:Username' is required");
        var password = rabbitSection["Password"] ?? throw new InvalidOperationException("Configuration 'RabbitMQ:Password' is required");

        services.AddMassTransit(bus =>
        {
            bus.SetKebabCaseEndpointNameFormatter();
            bus.AddConsumer<GenerateReportConsumer>();
            bus.AddConsumer<GenerateReportFaultConsumer>();
            bus.AddConsumer<UserAccountDeletedConsumer>();

            bus.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(host, "/", h =>
                {
                    h.Username(username);
                    h.Password(password);
                });

                cfg.UseMessageRetry(r => r.Intervals(
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(15)));

                cfg.ConfigureEndpoints(context);
            });
        });
    }
}

using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Toolkit.MessageBroker;

public static class BrokerStarter
{
    public static IHostBuilder AddConsumers<T>(this IHostBuilder builder, string variableName = "RabbitMq")
        where T : BrokerConsumerFactory, new()
    {
        if (builder == null)
            throw new ArgumentNullException("Builder not provided. Unable to start consumer target host.");
        if (variableName.IsEmpty())
            throw new ArgumentNullException("Broker variable name not provided. Unable to start consumer target host.");
        string host = Environment.GetEnvironmentVariable(variableName);
        if (host.IsEmpty())
            throw new ArgumentNullException("Unable to identify consumer target host.");
        return builder.ConfigureServices((context, collection) =>
            {
                collection.AddMassTransit(busRegistration =>
                {
                    var factory = new T();
                    factory.RegisterConsumers(busRegistration);
                    busRegistration.UsingRabbitMq((ctx, cfg) =>
                    {
                        cfg.Host(host);
                    });
                });
            });
    }

    public static void AddProducers(this IServiceCollection services, string variableName = "RabbitMq")
    {
        string host = Environment.GetEnvironmentVariable(variableName);
        if (host.IsEmpty())
            throw new ArgumentNullException("Unable to identify consumer target host.");
        services.AddMassTransit(busRegistration =>
        {
            busRegistration.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(host);
                cfg.UseMessageRetry(retry => { retry.Interval(3, TimeSpan.FromSeconds(5)); });
            });
        });
    }
}
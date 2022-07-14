using MassTransit;
using Toolkit.Register;
using Microsoft.Extensions.DependencyInjection;

namespace Toolkit.MessageBroker;

public static class BrokerStarter
{
    public static IServiceCollection AddConsumers<T>(this IServiceCollection services, string variableName = "RABBIT_MQ")
        where T : ResourcesFactory, new()
    {
        if (services == null)
            throw new ArgumentNullException("Services Collection not provided. Unable to start consumer target host.");
        if (variableName.IsEmpty())
            throw new ArgumentNullException("Broker variable name not provided. Unable to start consumer target host.");
        string host = Environment.GetEnvironmentVariable(variableName);
        if (host.IsEmpty())
            throw new ArgumentNullException("Unable to identify consumer target host.");
        services.AddMassTransit(busRegistration =>
        {
            var factory = new T();
            factory.RegisterConsumers(services, busRegistration);
            busRegistration.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(host);
                cfg.ConfigureEndpoints(ctx);
            });
        });
        return services;
    }
}
using MassTransit;
using Toolkit.Register;
using Microsoft.Extensions.DependencyInjection;
using Toolkit.TransactionalOutBox;

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

    public static IServiceCollection AddProducers<T>(this IServiceCollection services, TransactionOutboxType type,
        string transactionOutboxDatabaseConnection = "DATABASE_CONNECTION", string variableName = "RABBIT_MQ")
        where T : TransactionalOutBoxDbContext
    {
        //if (services == null)
        //    throw new ArgumentNullException("Services Collection not provided. Unable to start consumer target host.");
        //if (variableName.IsEmpty())
        //    throw new ArgumentNullException("Broker variable name not provided. Unable to start consumer target host.");
        //if (transactionOutboxDatabaseConnection.IsEmpty())
        //    throw new ArgumentNullException("Transaction Outbox Database variable name not provided. Unable to start consumer target host.");
        //string host = Environment.GetEnvironmentVariable(variableName);
        //if (host.IsEmpty())
        //    throw new ArgumentNullException("Unable to identify consumer target host.");
        //if (type != TransactionOutboxType.Postgres)
        //    throw new NotImplementedException($"Transactional Outbox Environment database {type} not implemented yet.");
        //string databaseConnection = Environment.GetEnvironmentVariable(transactionOutboxDatabaseConnection);
        //if (databaseConnection.IsEmpty())
        //    throw new ArgumentNullException("Unable to identify consumer target host.");
        //services.AddScoped<IRegistrationService, RegistrationService>();
        //services.AddMassTransit(busRegistration =>
        //{
        //    busRegistration.UsingRabbitMq((ctx, cfg) =>
        //    {
        //        cfg.Host(host);
        //        cfg.UseMessageRetry(retry => { retry.Interval(3, TimeSpan.FromSeconds(5)); });
        //        cfg.ConfigureEndpoints(ctx);
        //    });
        //});
        //services.AddPostgres<T>(transactionOutboxDatabaseConnection);
        return services;
    }
}
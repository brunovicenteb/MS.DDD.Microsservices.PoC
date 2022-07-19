using MassTransit;
using Toolkit.TransactionalOutBox;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Toolkit.OutBox.Consumer;

internal class ConsumerOutBoxStarter<T> : OutBoxStarter where T : OutBoxDbContext, new()
{
    internal ConsumerOutBoxStarter(WebApplicationBuilder builder, DatabaseType dbType,
        string dbConnectionVariableName = "DATABASE_CONNECTION")
        : base(builder, dbType, dbConnectionVariableName)
    {
    }

    protected override void DoUseDatabase(string stringConnection)
    {
        Builder.Services.AddDbContext<T>(o =>
        {
            if (OutBoxDbContext.DbType == DatabaseType.SqlServer)
                UseSqlServer(stringConnection, o);
            else
                UsePostgress(stringConnection, o);
        });
    }

    protected override void DoUseRabbitMq(string host)
    {
        Builder.Services.AddMassTransit(busRegistration =>
        {
            busRegistration.AddEntityFrameworkOutbox<T>(o =>
            {
                if (OutBoxDbContext.DbType == DatabaseType.SqlServer)
                    o.UseSqlServer();
                else
                    o.UsePostgres();
                o.DuplicateDetectionWindow = TimeSpan.FromSeconds(30);
            });
            busRegistration.SetKebabCaseEndpointNameFormatter();
            var context = new T();
            context.RegisterConsumers(Builder.Services, busRegistration);
            busRegistration.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(host);
                cfg.ConfigureEndpoints(context);
            });
        });
    }

    private void UsePostgress(string stringConnection, DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(stringConnection, options =>
        {
            options.MinBatchSize(1);
        });
    }

    private void UseSqlServer(string stringConnection, DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(stringConnection, options =>
        {
            options.MinBatchSize(1);
        });
    }
}
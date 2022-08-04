using MassTransit;
using System.Reflection;
using Toolkit.TransactionalOutBox;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.DependencyInjection;

namespace Toolkit.OutBox.Producer;

internal class ProducerOutBoxStarter<T> : OutBoxStarter where T : OutBoxDbContext
{
    internal ProducerOutBoxStarter(WebApplicationBuilder builder, string dbTypeVarName, bool recreateDb, string dbConnectionVarName)
        : base(builder, dbTypeVarName, dbConnectionVarName)
    {
        _RecreateDB = recreateDb;
    }

    private readonly bool _RecreateDB;

    protected override string TelemetryName => "producer";

    protected override void DoUseDatabase(string stringConnection)
    {
        Builder.Services.AddDbContext<T>(o =>
        {
            switch (OutBoxDbContext.DbType)
            {
                case DatabaseType.InMemory:
                    UseInMemory(stringConnection, o);
                    break;
                case DatabaseType.SqlServer:
                    UseSqlServer(stringConnection, o);
                    break;
                case DatabaseType.Postgres:
                    UsePostgress(stringConnection, o);
                    break;
                default:
                    throw new NotImplementedException($"DbType {OutBoxDbContext.DbType} not supported yet.");
            }
        });
        Builder.Services.AddHostedService(o => new RecreateDbHostedService<T>(_RecreateDB, o));
    }

    protected override void DoUseRabbitMq(string host)
    {
        Builder.Services.AddMassTransit(busRegistration =>
        {
            busRegistration.SetKebabCaseEndpointNameFormatter();
            busRegistration.AddEntityFrameworkOutbox<T>(o =>
            {
                o.QueryDelay = TimeSpan.FromSeconds(1);
                switch (OutBoxDbContext.DbType)
                {
                    case DatabaseType.SqlServer:
                        o.UseSqlServer();
                        break;
                    case DatabaseType.Postgres:
                        o.UsePostgres();
                        break;
                    default:
                        throw new NotImplementedException($"DbType {OutBoxDbContext.DbType} not supported yet.");
                }
                o.UseBusOutbox();
            });
            busRegistration.UsingRabbitMq((_, cfg) =>
            {
                cfg.Host(host);
                cfg.AutoStart = true;
            });
        });
    }

    protected override void DoUseHarness()
    {
        Builder.Services.AddMassTransitTestHarness(busRegistration =>
        {
            if (OutBoxDbContext.DbType == DatabaseType.InMemory)
                busRegistration.AddInMemoryInboxOutbox();
            else
            {
                busRegistration.AddEntityFrameworkOutbox<T>(o =>
                {
                    o.QueryDelay = TimeSpan.FromSeconds(1);
                    if (OutBoxDbContext.DbType == DatabaseType.SqlServer)
                        o.UseSqlServer();
                    else
                        o.UsePostgres();
                    o.UseBusOutbox();
                });
            }
        });
    }

    private void UseInMemory(string stringConnection, DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase(stringConnection);
    }

    private void UsePostgress(string stringConnection, DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(stringConnection, options =>
        {
            options.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
            options.MigrationsHistoryTable($"__{nameof(T)}");
            options.EnableRetryOnFailure(5);
            options.MinBatchSize(1);
        });
    }

    private void UseSqlServer(string stringConnection, DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(stringConnection, options =>
        {
            options.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
            options.MigrationsHistoryTable($"__{nameof(T)}");
            options.EnableRetryOnFailure(5);
            options.MinBatchSize(1);
        });
    }
}
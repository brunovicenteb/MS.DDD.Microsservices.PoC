using MassTransit;
using System.Reflection;
using Toolkit.TransactionalOutBox;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Toolkit.OutBox.Producer;

internal class ProducerOutBoxStarter<T> : OutBoxStarter where T : OutBoxDbContext
{
    internal ProducerOutBoxStarter(WebApplicationBuilder builder, DatabaseType dbType,
        bool recreateDb = false, string dbConnectionVarName = "DATABASE_CONNECTION")
        : base(builder, dbType, dbConnectionVarName)
    {
        _RecreateDB = recreateDb;
    }

    private readonly bool _RecreateDB;

    protected override void DoUseDatabase(string stringConnection)
    {
        Builder.Services.AddDbContext<T>(o =>
        {
            if (OutBoxDbContext.DbType == DatabaseType.SqlServer)
                UseSqlServer(stringConnection, o);
            else
                UsePostgress(stringConnection, o);
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
                if (OutBoxDbContext.DbType == DatabaseType.SqlServer)
                    o.UseSqlServer();
                else
                    o.UsePostgres();
                o.UseBusOutbox();
            });
            busRegistration.UsingRabbitMq((_, cfg) =>
            {
                cfg.Host(host);
                cfg.AutoStart = true;
            });
        });
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
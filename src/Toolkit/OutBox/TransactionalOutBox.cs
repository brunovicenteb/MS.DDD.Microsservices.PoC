using Serilog;
using MassTransit;
using Serilog.Events;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Toolkit.OutBox.Interfaces;

namespace Toolkit.TransactionalOutBox;

internal class TransactionalOutBox<T> : ILogable, IOpenTelemetreable, IDatabaseable, IBrokeable where T : TransactionalOutBoxDbContext
{
    internal TransactionalOutBox(WebApplicationBuilder builder)
    {
        _Builder = builder;
    }

    private readonly WebApplicationBuilder _Builder;
    private DatabaseType _DbType;
    private Type _DbContext;

    public IBrokeable UseDatabase(DatabaseType databaseType, bool recreateDatabase = false, string dbConnectionVariableName = "DATABASE_CONNECTION")
    {
        _DbType = databaseType;
        var db = databaseType == DatabaseType.SqlServer ? "SqlServer" : "Postgress";
        if (dbConnectionVariableName.IsEmpty())
            throw new ArgumentNullException($"{db} Connection variable name not provided. Unable to start consumer target host.");
        var stringConnection = Environment.GetEnvironmentVariable(dbConnectionVariableName);
        if (stringConnection.IsEmpty())
            throw new ArgumentNullException($"Unable to identify {db} Connection on {dbConnectionVariableName} variable. Unable to start Transactional OutBox.");
        _DbContext = typeof(T);
        _Builder.Services.AddDbContext<T>(o =>
        {
            if (databaseType == DatabaseType.SqlServer)
                UseSqlServer(stringConnection, o);
            else
                UsePostgress(stringConnection, o);
        });
        _Builder.Services.AddHostedService(o => new RecreateDbHostedService<T>(recreateDatabase, o));
        return this;
    }

    public void UseRabbitMq(string rabbitMqVariableName = "RABBIT_MQ")
    {
        if (rabbitMqVariableName.IsEmpty())
            throw new ArgumentNullException("RabbitMq variable name not provided. Unable to start consumer target host.");
        string host = Environment.GetEnvironmentVariable(rabbitMqVariableName);
        if (host.IsEmpty())
            throw new ArgumentNullException($"Unable to identify RabbitMq Host on {rabbitMqVariableName} variable. Unable to start Transactional OutBox.");
        _Builder.Services.AddMassTransit(x =>
        {
            x.AddEntityFrameworkOutbox<T>(o =>
            {
                o.QueryDelay = TimeSpan.FromSeconds(1);
                if (_DbType == DatabaseType.SqlServer)
                    o.UseSqlServer();
                else
                    o.UsePostgres();
                o.UseBusOutbox();
            });
            x.UsingRabbitMq((_, cfg) =>
            {
                cfg.Host(host);
                cfg.AutoStart = true;
            });
        });
    }

    public IOpenTelemetreable UseSerilog()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("MassTransit", LogEventLevel.Debug)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();
        _Builder.Host.UseSerilog();
        return this;
    }

    public IDatabaseable UseOpenTelemetry()
    {
        return this;
    }

    public IDatabaseable DoNotOpenTelemetry()
    {
        return this;
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
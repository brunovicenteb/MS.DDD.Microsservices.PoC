using Serilog;
using Serilog.Events;
using Toolkit.TransactionalOutBox;
using Microsoft.AspNetCore.Builder;

namespace Toolkit.OutBox;

internal abstract class OutBoxStarter : ILogable, IOpenTelemetreable, IDatabaseable, IBrokeable
{
    internal OutBoxStarter(WebApplicationBuilder builder, DatabaseType dbType,
        string dbConnectionVarName = "DATABASE_CONNECTION")
    {
        Builder = builder;
        _DbConnectionVarName = dbConnectionVarName;
        OutBoxDbContext.SetDbType(dbType);
    }

    protected readonly WebApplicationBuilder Builder;
    private readonly string _DbConnectionVarName;

    protected abstract void DoUseDatabase(string stringConnection);

    protected abstract void DoUseRabbitMq(string host);

    public IBrokeable UseDatabase()
    {
        var db = OutBoxDbContext.DbType == DatabaseType.SqlServer ? "SqlServer" : "Postgress";
        if (_DbConnectionVarName.IsEmpty())
            throw new ArgumentNullException($"{db} Connection variable name not provided. Unable to start consumer target host.");
        var stringConnection = Environment.GetEnvironmentVariable(_DbConnectionVarName);
        if (stringConnection.IsEmpty())
            throw new ArgumentNullException($"Unable to identify {db} Connection on {_DbConnectionVarName} variable. Unable to start Transactional OutBox.");
        DoUseDatabase(stringConnection);
        return this;
    }

    public void UseRabbitMq(string rabbitMqVariableName = "RABBIT_MQ")
    {
        if (rabbitMqVariableName.IsEmpty())
            throw new ArgumentNullException("RabbitMq variable name not provided. Unable to start consumer target host.");
        string host = Environment.GetEnvironmentVariable(rabbitMqVariableName);
        if (host.IsEmpty())
            throw new ArgumentNullException($"Unable to identify RabbitMq Host on {rabbitMqVariableName} variable. Unable to start Transactional OutBox.");
        DoUseRabbitMq(host);
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
        Builder.Host.UseSerilog();
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
}
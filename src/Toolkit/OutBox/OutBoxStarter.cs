using Serilog;
using OpenTelemetry;
using Serilog.Events;
using System.Diagnostics;
using OpenTelemetry.Trace;
using MassTransit.Metadata;
using OpenTelemetry.Resources;
using Toolkit.TransactionalOutBox;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Toolkit.OutBox;

internal abstract class OutBoxStarter : ILogable, IOpenTelemetreable, IDatabaseable, IBrokeable
{
    internal OutBoxStarter(WebApplicationBuilder builder, string dbTypeVarName, string dbConnectionVarName)
    {
        Builder = builder;
        _DbConnectionVarName = dbConnectionVarName;
        _DbTypeVarName = dbTypeVarName;
    }

    protected readonly WebApplicationBuilder Builder;
    private readonly string _DbTypeVarName;
    private readonly string _DbConnectionVarName;

    protected abstract string TelemetryName { get; }

    protected abstract void DoUseDatabase(string stringConnection);

    protected abstract void DoUseRabbitMq(string host);

    public IBrokeable UseDatabase()
    {
        if (_DbTypeVarName.IsEmpty())
            throw new ArgumentNullException($"DbType variable name not provided. Unable to start consumer target host.");
        var strDbType = Environment.GetEnvironmentVariable(_DbTypeVarName);
        if (strDbType.IsEmpty())
            throw new ArgumentNullException($"Unable to identify DbType on {_DbTypeVarName} variable. Unable to start Transactional OutBox.");
        DatabaseType dbType;
        if (!Enum.TryParse(strDbType, out dbType))
            throw new ArgumentNullException($"Invalid DbType ({strDbType}) informed on {_DbTypeVarName} variable. Unable to start Transactional OutBox.");
        OutBoxDbContext.SetDbType(dbType);
        var db = Enum.GetName(OutBoxDbContext.DbType);
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
        Builder.Services.AddOpenTelemetryTracing(builder =>
        {
            builder.SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService(TelemetryName)
                    .AddTelemetrySdk()
                    .AddEnvironmentVariableDetector())
                .AddSource("MassTransit")
                .AddAspNetCoreInstrumentation()
                .AddJaegerExporter(o =>
                {
                    o.AgentHost = HostMetadataCache.IsRunningInContainer ? "jaeger" : "localhost";
                    o.AgentPort = 6831;
                    o.MaxPayloadSizeInBytes = 4096;
                    o.ExportProcessorType = ExportProcessorType.Batch;
                    o.BatchExportProcessorOptions = new BatchExportProcessorOptions<Activity>
                    {
                        MaxQueueSize = 2048,
                        ScheduledDelayMilliseconds = 5000,
                        ExporterTimeoutMilliseconds = 30000,
                        MaxExportBatchSize = 512,
                    };
                });
        });
        return this;
    }

    public IDatabaseable DoNotOpenTelemetry()
    {
        return this;
    }
}
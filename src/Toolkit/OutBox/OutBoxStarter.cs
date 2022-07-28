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

internal abstract class OutBoxStarter : ILogable, ITelemetreable, IDatabaseable, IBrokeable
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
    protected abstract void DoUseHarness();

    public IBrokeable UseDatabase()
    {
        var strDbType = EnvironmentReader.Read<string>(_DbTypeVarName, varEmptyError:
            $"Unable to identify DbType on {_DbTypeVarName} variable. Unable to start Transactional OutBox.");
        DatabaseType dbType;
        if (!Enum.TryParse(strDbType, out dbType))
            throw new ArgumentNullException($"Invalid DbType ({strDbType}) informed on {_DbTypeVarName} variable. Unable to start Transactional OutBox.");
        OutBoxDbContext.SetDbType(dbType);
        var db = Enum.GetName(OutBoxDbContext.DbType);
        var stringConnection = EnvironmentReader.Read<string>(_DbConnectionVarName, varEmptyError:
            $"Unable to identify {db} Connection on {_DbConnectionVarName} variable. Unable to start Transactional OutBox.");
        DoUseDatabase(stringConnection);
        return this;
    }

    public void UseRabbitMq(string rabbitMqVariableName = "RABBIT_MQ")
    {
        var host = EnvironmentReader.Read<string>(rabbitMqVariableName, varEmptyError:
            $"Unable to identify RabbitMq Host on {rabbitMqVariableName} variable. Unable to start Transactional OutBox.");
        DoUseRabbitMq(host);
    }

    public void UseHarness()
    {
        DoUseHarness();
    }

    public ITelemetreable UseSerilog()
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

    public IDatabaseable UseTelemetry(string telemetryHost = "TELEMETRY_HOST")
    {
        var host = EnvironmentReader.Read<string>(telemetryHost, varEmptyError:
            $"Unable to identify Open Telemetry Host on {telemetryHost} variable. Unable to start Transactional OutBox.");
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
                    o.AgentHost = host;
                    o.AgentPort = EnvironmentReader.Read("TELEMETRY_AGENT_PORT", 6831);
                    o.MaxPayloadSizeInBytes = EnvironmentReader.Read("TELEMETRY_MAX_PAY_LOAD_SIZE_IN_BYTES", 4096);
                    o.ExportProcessorType = ExportProcessorType.Batch;
                    o.BatchExportProcessorOptions = new BatchExportProcessorOptions<Activity>
                    {
                        MaxQueueSize = EnvironmentReader.Read("TELEMETRY_BATCH_MAX_QUEUE_SIZE", 2048),
                        ScheduledDelayMilliseconds = EnvironmentReader.Read("TELEMETRY_BATCH_SCHEDULED_DELAY__MILLISECONDS", 5000),
                        ExporterTimeoutMilliseconds = EnvironmentReader.Read("TELEMETRY_BATCH_EXPORTER_TIMEOUT_MILLISECONDS", 30000),
                        MaxExportBatchSize = EnvironmentReader.Read("TELEMETRY_BATCH_MAX_EXPORT_BATCH_SIZE", 512)
                    };
                });
        });
        return this;
    }

    public IDatabaseable DoNotUseTelemetry()
    {
        return this;
    }

    public IBrokeable DoNotUseDatabase()
    {
        return this;
    }
}
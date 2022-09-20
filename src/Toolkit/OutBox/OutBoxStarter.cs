using Serilog;
using OpenTelemetry;
using Serilog.Events;
using System.Diagnostics;
using OpenTelemetry.Trace;
using Toolkit.MessageBroker;
using OpenTelemetry.Resources;
using Toolkit.TransactionalOutBox;
using Microsoft.AspNetCore.Builder;
using Toolkit.Authentication.OktaUtils;
using Microsoft.Extensions.DependencyInjection;

namespace Toolkit.OutBox;

internal abstract class OutBoxStarter : ILogable, ITelemetreable, IDatabaseable, IBrokeable, IAuthenticable
{
    internal OutBoxStarter(WebApplicationBuilder builder, string dbTypeVarName, string dbConnectionVarName,
        string retryCountVarName, string retryIntevalInMillisecondsVarName)
    {
        Builder = builder;
        _DbTypeVarName = dbTypeVarName;
        _DbConnectionVarName = dbConnectionVarName;
        int retryCount = EnvironmentReader.Read(retryCountVarName, defaultValue: 5);
        int retryIntevalInMilliseconds = EnvironmentReader.Read(retryIntevalInMillisecondsVarName, defaultValue: 100);
        BrokerConsumer.SetRetryParameters(retryCount, retryIntevalInMilliseconds);
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
        if (!Enum.TryParse(strDbType, out DatabaseType dbType))
            throw new ArgumentNullException($"Invalid DbType ({strDbType}) informed on {_DbTypeVarName} variable. Unable to start Transactional OutBox.");
        OutBoxDbContext.SetDbType(dbType);
        var db = Enum.GetName(OutBoxDbContext.DbType);
        var stringConnection = EnvironmentReader.Read<string>(_DbConnectionVarName, varEmptyError:
            $"Unable to identify {db} Connection on {_DbConnectionVarName} variable. Unable to start Transactional OutBox.");
        DoUseDatabase(stringConnection);
        return this;
    }

    public IAuthenticable UseRabbitMq(string rabbitMqVariableName = "RABBIT_MQ")
    {
        var host = EnvironmentReader.Read<string>(rabbitMqVariableName, varEmptyError:
            $"Unable to identify RabbitMq Host on {rabbitMqVariableName} variable. Unable to start Transactional OutBox.");
        DoUseRabbitMq(host);
        return this;
    }

    public IAuthenticable UseHarness()
    {
        DoUseHarness();
        return this;
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

    public void UseOkta(string clientIdVarName = "OKTA_CLIENT_ID", string clienteSecretVarName = "OKTA_SECRET",
        string domainVarName = "OKTA_DOMAIN", string autorizationServerIdVarName = "OKTA_SERVER_ID",
        string audienceVarName = "OKTA_AUDIENCE")
    {
        var clientId = EnvironmentReader.Read<string>(clientIdVarName, varEmptyError:
            $"Unable to identify OktaClientId on {clientIdVarName} variable. Unable to start Transactional OutBox.");
        var secret = EnvironmentReader.Read<string>(clienteSecretVarName, varEmptyError:
            $"Unable to identify OktaSecret on {clienteSecretVarName} variable. Unable to start Transactional OutBox.");
        var oktaDomain = EnvironmentReader.Read<string>(domainVarName, varEmptyError:
            $"Unable to identify OktaDomain on {domainVarName} variable. Unable to start Transactional OutBox.");
        var oktaAutorizationServerId = EnvironmentReader.Read<string>(autorizationServerIdVarName, varEmptyError:
            $"Unable to identify OktaAutorizationServerId on {autorizationServerIdVarName} variable. Unable to start Transactional OutBox.");
        var oktaAudience = EnvironmentReader.Read<string>(audienceVarName, varEmptyError:
            $"Unable to identify OktaAudience on {audienceVarName} variable. Unable to start Transactional OutBox.");
        OktaBuilder.AddSecurityDefinition(Builder.Services, clientId, secret, oktaDomain, oktaAutorizationServerId, oktaAudience);
    }

    public void NotUseAuthentication()
    {
    }
}
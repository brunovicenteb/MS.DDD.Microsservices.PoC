using Toolkit.OutBox;
using Toolkit.TransactionalOutBox;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Benefit.Test;

public abstract class StarterIoC<TDbContext> : IDisposable
    where TDbContext : OutBoxDbContext, new()
{
    private ServiceProvider CreateProvider()
    {
        var dbType = DatabaseType.InMemory.ToString();
        Environment.SetEnvironmentVariable("DATABASE_TYPE", dbType);
        Environment.SetEnvironmentVariable("DATABASE_CONNECTION", dbType);
        Environment.SetEnvironmentVariable("RETRY_COUNT", "1");
        Environment.SetEnvironmentVariable("RETRY_INTERVAL_IN_MILLISECONDS", "1");
        Environment.SetEnvironmentVariable("TELEMETRY_HOST", "jaeger");
        Environment.SetEnvironmentVariable("JAEGER_AGENT_HOST", "jaeger");
        Environment.SetEnvironmentVariable("JAEGER_AGENT_PORT", "6831");
        Environment.SetEnvironmentVariable("JAEGER_SAMPLER_TYPE", "remote");
        Environment.SetEnvironmentVariable("JAEGER_SAMPLING_ENDPOINT", "http://jaeger:5778/sampling");
        var builder = WebApplication.CreateBuilder();
        builder.BeginProducer<TDbContext>()
            .UseSerilog()
            .UseTelemetry()
            .UseDatabase()
            .UseHarness();
        builder.BeginConsumer<TDbContext>()
            .UseSerilog()
            .UseTelemetry()
            .UseDatabase()
            .UseHarness();
        DoRegisterResources(builder);
        return builder.Services.BuildServiceProvider();
    }

    protected virtual void DoRegisterResources(WebApplicationBuilder builder)
    {
    }

    public StarterIoC()
    {
        Provider = CreateProvider();
    }

    internal ServiceProvider Provider;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool isDisposing)
    {
        if (!isDisposing)
            return;
        DoDispose();
    }

    protected async virtual void DoDispose()
    {
        Environment.SetEnvironmentVariable("DATABASE_TYPE", string.Empty);
        Environment.SetEnvironmentVariable("DATABASE_CONNECTION", string.Empty);
        Environment.SetEnvironmentVariable("TELEMETRY_HOST", string.Empty);
        Environment.SetEnvironmentVariable("JAEGER_AGENT_HOST", string.Empty);
        Environment.SetEnvironmentVariable("JAEGER_AGENT_PORT", string.Empty);
        Environment.SetEnvironmentVariable("JAEGER_SAMPLER_TYPE", string.Empty);
        Environment.SetEnvironmentVariable("JAEGER_SAMPLING_ENDPOINT", string.Empty);
        await Provider.DisposeAsync();
    }
}
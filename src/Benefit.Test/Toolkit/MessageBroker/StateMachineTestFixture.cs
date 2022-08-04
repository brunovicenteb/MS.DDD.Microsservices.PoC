using MassTransit;
using Toolkit.OutBox;
using MassTransit.Testing;
using Toolkit.TransactionalOutBox;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Benefit.Test.Toolkit.MessageBroker;

public class StateMachineTestFixtureFixture<TDbContext> : IDisposable
    where TDbContext : OutBoxDbContext, new()
{
    private ServiceProvider CreateProvider()
    {
        var dbType = DatabaseType.InMemory.ToString();
        Environment.SetEnvironmentVariable("DATABASE_TYPE", dbType);
        Environment.SetEnvironmentVariable("DATABASE_CONNECTION", dbType);
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
        return builder.Services.BuildServiceProvider();
    }

    public StateMachineTestFixtureFixture()
    {
        Provider = CreateProvider();
        TestHarness = Provider.GetRequiredService<ITestHarness>();
        TestHarness.Start();
    }

    internal ServiceProvider Provider;
    internal ITestHarness TestHarness;

    public async void Dispose()
    {
        Environment.SetEnvironmentVariable("DATABASE_TYPE", string.Empty);
        Environment.SetEnvironmentVariable("DATABASE_CONNECTION", string.Empty);
        Environment.SetEnvironmentVariable("TELEMETRY_HOST", string.Empty);
        Environment.SetEnvironmentVariable("JAEGER_AGENT_HOST", string.Empty);
        Environment.SetEnvironmentVariable("JAEGER_AGENT_PORT", string.Empty);
        Environment.SetEnvironmentVariable("JAEGER_SAMPLER_TYPE", string.Empty);
        Environment.SetEnvironmentVariable("JAEGER_SAMPLING_ENDPOINT", string.Empty);
        await TestHarness?.Stop();
        await Provider.DisposeAsync();
    }
}

public class StateMachineTestFixture<TDbContext, TStateMachine, TInstance> : IClassFixture<StateMachineTestFixtureFixture<TDbContext>>
    where TDbContext : OutBoxDbContext, new()
    where TStateMachine : class, SagaStateMachine<TInstance>
    where TInstance : class, SagaStateMachineInstance
{
    protected readonly TStateMachine Machine;
    protected readonly ServiceProvider Provider;
    protected readonly ISagaStateMachineTestHarness<TStateMachine, TInstance> SagaHarness;
    protected readonly ITestHarness TestHarness;

    public StateMachineTestFixture(StateMachineTestFixtureFixture<TDbContext> fixture)
    {
        Provider = fixture.Provider;
        TestHarness = Provider.GetRequiredService<ITestHarness>();
        SagaHarness = TestHarness.GetSagaStateMachineHarness<TStateMachine, TInstance>();
        Machine = SagaHarness.StateMachine;
    }
}
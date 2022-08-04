using MassTransit;
using Toolkit.Mapper;
using Toolkit.OutBox;
using Toolkit.Interfaces;
using MassTransit.Testing;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Benefit.Test.Toolkit.MessageBroker.Mock;
using Benefit.Test.Toolkit.MessageBroker.Mock.Sagas;
using Benefit.Test.Toolkit.MessageBroker.Mock.Sagas.Contract;
using Benefit.Test.Toolkit.MessageBroker.Mock.Sagas.Contract.States;

namespace Benefit.Test.Toolkit.MessageBroker;

public class MessageBrokerTest
{
    [Fact]
    public async void TestePublishAndConsumeTestMessage()
    {
        //arrange
        //Environment.SetEnvironmentVariable("TELEMETRY_HOST", "jaeger");
        //Environment.SetEnvironmentVariable("JAEGER_AGENT_HOST", "jaeger");
        //Environment.SetEnvironmentVariable("JAEGER_AGENT_PORT", "6831");
        //Environment.SetEnvironmentVariable("JAEGER_SAMPLER_TYPE", "remote");
        //Environment.SetEnvironmentVariable("JAEGER_SAMPLING_ENDPOINT", "http://jaeger:5778/sampling");

        var builder = WebApplication.CreateBuilder();
        builder.BeginProducer<ObjectContextMock>()
            .UseSerilog()
            .UseTelemetry()
            .UseDatabase();
            /*.UseHarness()*/;
        builder.BeginConsumer<ObjectContextMock>()
            .UseSerilog()
            .UseTelemetry()
            .DoNotUseDatabase()
            .UseHarness();
        //var builder = new ServiceCollection()
        //.AddMassTransitTestHarness(cfg =>
        //{
        //    cfg.AddConsumer<ObjectCreatedConsumerMock>();
        //    cfg.AddSagaStateMachine<ObjectStateMachineMock, ObjectStateMock>();
        //});
        var serviceProvider = builder.Services.BuildServiceProvider(true);
        var harness = serviceProvider.GetRequiredService<ITestHarness>();

        await harness.Start();

        IGenericMapper genericMapping = MapperFactory.Map<ObjectMock, ObjectSubmittedMock>();

        ObjectMock objectMock = new ObjectMock()
        {
            Name = "Test Event"
        };

        var objectSubmittedMock= genericMapping.Map<ObjectMock, ObjectSubmittedMock>(objectMock);
        var sagaCorrelationId = NewId.NextGuid();
        objectSubmittedMock.CorrelationId = sagaCorrelationId;
        
        //act
        await harness.Bus.Publish(objectSubmittedMock);

        bool consumedMessage = await harness.Consumed.Any<ObjectSubmittedMock>();

        var sagaHarness = harness.GetSagaStateMachineHarness<ObjectStateMachineMock, ObjectStateMock>();
        var machine = sagaHarness.StateMachine;

        var consumedSaga = await sagaHarness.Consumed.Any<ObjectSubmittedMock>();
        var createdSaga = await sagaHarness.Created.Any(x => x.CorrelationId == sagaCorrelationId);

        //assert
        bool publishedMessage = await sagaHarness.Consumed.Any<ObjectSubmittedMock>();
        bool createdSagaSameCorrelationId = await sagaHarness.Created.Any(x => x.CorrelationId == sagaCorrelationId);

        Assert.True(publishedMessage);
        Assert.True(createdSagaSameCorrelationId);
    }
}

using Benefit.Service.Workers;
using Benefit.Test.Toolkit.MessageBroker.Mock;
using Benefit.Test.Toolkit.MessageBroker.Mock.Infra;
using Benefit.Test.Toolkit.MessageBroker.Mock.Sagas;
using Benefit.Test.Toolkit.MessageBroker.Mock.Sagas.Contract;
using Benefit.Test.Toolkit.MessageBroker.Mock.Sagas.Contract.States;
using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using Toolkit.Interfaces;
using Toolkit.Mapper;
using Toolkit.MessageBroker;
using Toolkit.OutBox;

namespace Benefit.Test.Toolkit.MessageBroker
{
    public class MessageBrokerTest
    {
        [Fact]
        public async void TestePublishAndConsumeTestMessage()
        {
            //arrange
            //Environment.SetEnvironmentVariable("DATABASE_TYPE", "Postgres");
            //Environment.SetEnvironmentVariable("DATABASE_CONNECTION", "Host=postgres;Database=IntegrationTest;Port=5432;Username=guest;Password=guest");
            Environment.SetEnvironmentVariable("TELEMETRY_HOST", "jaeger");
            Environment.SetEnvironmentVariable("JAEGER_AGENT_HOST", "jaeger");
            Environment.SetEnvironmentVariable("JAEGER_AGENT_PORT", "6831");
            Environment.SetEnvironmentVariable("JAEGER_SAMPLER_TYPE", "remote");
            Environment.SetEnvironmentVariable("JAEGER_SAMPLING_ENDPOINT", "http://jaeger:5778/sampling");

            var builder = WebApplication.CreateBuilder();
            builder.BeginProducer<ObjectContextMock>()
                .UseSerilog()
                .UseTelemetry()
                .DoNotUseDatabase()
                /*.UseHarness()*/;
            builder.BeginConsumer<ObjectContextMock>()
                .UseSerilog()
                .UseTelemetry()
                .DoNotUseDatabase()
                .UseHarness();

            var serviceProvider = builder.Services.BuildServiceProvider();
            var repository = serviceProvider.GetRequiredService<ISagaRepository<ObjectStateMock>>();

            CancellationToken cancellationToken = new CancellationTokenSource(5000).Token;
            var harness = serviceProvider.GetRequiredService<ITestHarness>();

            harness.Start().Wait();
            var sagaHarness = harness.GetSagaStateMachineHarness<ObjectStateMachineMock, ObjectStateMock>();
            var machine = sagaHarness.StateMachine;

            IGenericMapper genericMapping = MapperFactory.Map<ObjectMock, ObjectSubmittedMock>();

            ObjectMock objectMock = new ObjectMock()
            {
                Name = "Test Event"
            };

            var objectSubmittedMock= genericMapping.Map<ObjectMock, ObjectSubmittedMock>(objectMock);
            var sagaCorrelationId = NewId.NextGuid();
            objectSubmittedMock.CorrelationId = sagaCorrelationId;
            //act

            //var repository = serviceProvider.GetRequiredService<ISagaRepository<ObjectStateMock>>();
            //Guid? existsIdBeforeSagaInit = await repository.ShouldContainSagaInState(objectMock.CorrelationId, machine, x => x.SubmittedState, harness.TestTimeout);

            //await repository.AddAsync(objectMock, false);
            harness.Bus.Publish(objectSubmittedMock).Wait();

            //Guid? existsIdAfterSagaInit = await repository.ShouldContainSagaInState(objectSubmittedMock.CorrelationId, machine, x => x.SubmittedState, harness.TestTimeout);
            var existsIdAfterSagaInit = await sagaHarness.Exists(objectSubmittedMock.CorrelationId, state => state.SubmittedState);
            //assert
            //Assert.True(existsIdBeforeSagaInit.HasValue, "Saga was not created using the MessageId");
            //Assert.True(existsIdAfterSagaInit.HasValue, "Saga did not transition to Ready");
            bool publishedMessage = await sagaHarness.Consumed.Any<ObjectSubmittedMock>();
            bool createdSagaSameCorrelationId = await sagaHarness.Created.Any(x => x.CorrelationId == sagaCorrelationId);

            Assert.True(publishedMessage);
            Assert.True(createdSagaSameCorrelationId);
        }
    }
}

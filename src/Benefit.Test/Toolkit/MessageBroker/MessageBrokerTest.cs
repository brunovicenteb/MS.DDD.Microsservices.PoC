using Benefit.Service.Workers;
using Benefit.Test.Toolkit.MessageBroker.Mock;
using Benefit.Test.Toolkit.MessageBroker.Mock.Sagas;
using Benefit.Test.Toolkit.MessageBroker.Mock.Sagas.Contract;
using Benefit.Test.Toolkit.MessageBroker.Mock.Sagas.Contract.States;
using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
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
            var builder = WebApplication.CreateBuilder();
            builder.BeginProducer<ObjectContextMock>();
            builder.BeginConsumer<ObjectContextMock>();

            var serviceProvider = builder.Services.BuildServiceProvider();

            CancellationToken cancellationToken = new CancellationTokenSource(5000).Token;
            var harness = serviceProvider.GetRequiredService<ITestHarness>();

            harness.Start().Wait();
            var sagaHarness = harness.GetSagaStateMachineHarness<ObjectStateMachineMock, ObjectStateMock>();
            var machine = sagaHarness.StateMachine;

            ObjectCreatedMock objectMock = new ObjectCreatedMock()
            {
                Name = "Test Event"
            };
            objectMock.CorrelationId = NewId.NextGuid();
            //act

            var repository = serviceProvider.GetRequiredService<ISagaRepository<ObjectStateMock>>();
            Guid? existsIdBeforeSagaInit = await repository.ShouldContainSagaInState(objectMock.CorrelationId, machine, x => x.SubmittedState, harness.TestTimeout);
           
            harness.Bus.Publish(objectMock).Wait();

            Guid? existsIdAfterSagaInit = await repository.ShouldContainSagaInState(objectMock.CorrelationId, machine, x => x.SubmittedState, harness.TestTimeout);
            //assert
            Assert.True(existsIdBeforeSagaInit.HasValue, "Saga was not created using the MessageId");
            Assert.True(existsIdAfterSagaInit.HasValue, "Saga did not transition to Ready");
        }
    }
}
